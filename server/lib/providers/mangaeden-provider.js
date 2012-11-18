/**
 * User: molant
 * Date: 11/16/12
 * Time: 10:32 PM
 */
//require database.js
"use strict";

var providerId = 'MangaEden',
    request = require('request'),
    promised = require("promised-io/promise"),
    Deferred = require('promised-io').Deferred,
    mangaDb = require('../helpers/database.js'),
    logger = require('tracer').console({
        format:"{{timestamp}} <{{title}}> {{message}} (in {{file}}:{{line}})",
        dateformat:"HH:MM:ss.L"
    }),
    fs = require('fs');

function requestParser(uri, callback) {
    var deferred = new Deferred();
    request(uri, function (error, response, body) {
        if (!error && response.statusCode === 200) {
            callback(body, deferred);
        } else {
            deferred.resolve(false);
        }
    });

    return deferred.promise;
}

function updateList() {
    var uri = 'http://www.mangaeden.com/api/list/0';
    logger.log('MangaEden - Downloading list');

    return requestParser(uri, function (content, promise) {
        //MangaEden returns a nice JSON
        var mangas = JSON.parse(content).manga;
        logger.log('MangaEden - List downloaded');
        //code to update the database somwhere here

        promise.resolve(mangas);
    });
}

function updateAllMangas(mangas) {
    //setting more workers caused warnings about possible memory leaks
    // in emitter (probably related to mongojs
    var workers = 5,
        i = 0,
        deferred = new Deferred();
    logger.log('MangaEden - Updating Mangas');


    function error(data) {
        logger.error('MangaEden - ' + data);
    }

    function next() {
        i++;
        if (i < mangas.length) {
            updateManga(mangas[i].i, i).then(next, error);
        } else {
            deferred.resolve(true);
        }
    }

    for (var j = 0; j < workers; j++) {
        next();
    }

    return deferred.promise;
}

function downloadChapter(info) {
    var uri = 'http://www.mangaeden.com/api/chapter/' + info[3];
    return requestParser(uri, function (content, promise) {
        var pages = JSON.parse(content).images,
            chapter = {
                externalId:info[3],
                number:info[0],
                uploadedDate:info[1],
                //we should download the pages here
                pages:[],
                title:''
            };

        for (var i = 0; i < pages.length; i++) {
            pages[i] = {
                number:pages[i][0],
                url:'http://cdn.mangaeden.com/mangasimg/' + pages[i][1]
            };
        }
        chapter.pages = pages;
        promise.resolve(chapter);
    });
}

function normalizeManga(manga) {
    var deferred = new Deferred();
    delete manga['baka'];
    delete manga['language'];
    delete manga['aka-alias'];
    delete manga['aka'];
    delete manga['type'];
    delete manga.imageURL;
    manga.authors = [manga.author];
    delete manga.author;
    manga.artists = [manga.artist];
    delete manga.artist;
    manga.image = 'http://cdn.mangaeden.com/mangasimg/' + manga.image;
    manga.alias = [manga.alias];

    //MangaEden chapter structure is something like:
    //[8, 734074, "8", "4e738898c09225616d2e5b65"]
    //first value is the number, second is the added date (days from year 1), third is again the number
    //in string format and finally the last is the id
    var chapters = [], i = -1;

    function next() {
        i++;
        if (i < manga.chapters.length) {
            downloadChapter(manga.chapters[i]).then(function (chapter) {
                chapters.push(chapter);
                next();
            });
        } else {
            manga.chapters = chapters;
            deferred.resolve(manga);
        }
    }

    next();

    return deferred.promise;
}

function getChapterPages(chapterId) {

}

function updateManga(externalId, index) {
    var uri = 'http://www.mangaeden.com/api/manga/' + externalId;

    return requestParser(uri, function (content, promise) {
        var manga = JSON.parse(content),
            chapters = manga.chapters;

        //we clean up all the information we don't need
        normalizeManga(manga).then(function(manga){
            mangaDb.addManga(manga, externalId, providerId).then(function (result) {
                //updateChapters with images here
                promise.resolve(result);
            });
        })
    });
}

function updateChapter(id) {

}

function update() {
    var actions = [updateList, updateAllMangas];

    return promised.seq(actions);
}

module.exports.update = update;