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
    // in emitter (probably related to mongojs)
    var workers = 5,
        i = 0,
        stopped = 0,
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
            stopped++;
            if (stopped === 5) {
                deferred.resolve(true);
            }
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
    //var deferred = new Deferred();
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
    manga.providers = [providerId];
    delete manga.chapters;

    //return deferred.promise;
}

function downloadChapters(mangaChapters) {
    var deferred = new Deferred();

    //MangaEden chapter structure is something like:
    //[8, 734074, "8", "4e738898c09225616d2e5b65"]
    //first value is the number, second is the added date (days from year 1), third is again the number
    //in string format and finally the last is the id
    var chapters = [], i = -1;

    //TODO: remove the chapters that are already in the db

    function next() {
        i++;
        if (i < mangaChapters.length) {
            downloadChapter(mangaChapters[i]).then(function (chapter) {
                chapters.push(chapter);
                next();
            });
        } else {
            deferred.resolve(chapters);
        }
    }

    next();

    return deferred.promise;
}

function updateManga(externalId, index) {
    var uri = 'http://www.mangaeden.com/api/manga/' + externalId;

    return requestParser(uri, function (content, promise) {
        var manga = JSON.parse(content),
            chapters = manga.chapters;

        //we clean up all the information we don't need and format some other
        normalizeManga(manga);

        mangaDb.addOrUpdateManga(manga, externalId, providerId)
            .then(function (result) {
                if (result.chapters) {
                    downloadChapters(chapters).then(function (chapters) {
                        mangaDb.addChapters(chapters, manga._id, providerId).then(function () {
                            logger.log('Inserted - %s', manga.title);
                            promise.resolve();
                        }, function (err) {
                            promise.reject(err);
                        });
                    });
                } else {
                    logger.log('Inserted - %s', manga.title);
                    promise.resolve();
                }
            }, function (err) {
                promise.reject(err);
            });
    });
}

function update() {
    var actions = [updateList, updateAllMangas];
    mangaDb.startUpdate().then(function(){
        //update version somewhere?
        return promised.seq(actions);
    });
}

module.exports.update = update;