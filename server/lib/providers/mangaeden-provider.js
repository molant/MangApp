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
    logger = require('tracer').console({
        format:"{{timestamp}} <{{title}}> {{message}}",
        dateformat:"HH:MM:ss.L"
    }),
    fs = require('fs'),
    mongojs = require('mongojs'),
    db = mongojs('mangapp', ['mangas', 'chapters', 'mangaMap', 'chapterMap']),
    mangas;

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
        mangas = JSON.parse(content).manga;
        logger.log('MangaEden - List downloaded');
        //code to update the database somwhere here

        promise.resolve(true);
    });
}

function updateAllMangas() {
    var workers = 6,
        i = 0,
        deferred = new Deferred();
    logger.log('MangaEden - Updating Mangas');


    function error(data) {
        logger.error('MangaEden - ' + data);
    }

    function next() {
        i++;
        if (i < mangas.length) {
            logger.log('MangaEden - Updating Manga %s', mangas[i].i);
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

function normalizeManga(manga) {
    delete manga['baka'];
    delete manga['language'];
    delete manga['aka-alias'];
    delete manga['aka'];
    delete manga['type'];
    manga.image = 'http://cdn.mangaeden.com/mangasimg/' + manga.image;
    delete manga.imageURL;
    delete manga.chapters;
}

function updateDB(manga) {
    //add timestamp to manga
    //check if manga exists
    db.mangaMap.find({externalId:manga.externalId, providerId:providerId}, function (err, docs) {
        if (!err) {
            if (docs.length > 0) {
                manga.id = docs[0].id;
            } else {
                db.mangaMap.save({id:manga.id, externalId:manga.externalId, providerId:providerId}, function (err, saved) {
                    if (err || !saved) {
                        logger.err('Error updating mangaMap for %s', manga.title);
                    }else{
                        logger.log('%s inserted into mangaMap collection', manga.title);
                    }
                });
            }

            //updaters
        }

        logger.log(err + ' ' + docs);
    });
    //insert or update
}

function updateManga(externalId, id) {
    var uri = 'http://www.mangaeden.com/api/manga/' + externalId;
    logger.log('MangaEden - Updating Manga %s', externalId);
    return requestParser(uri, function (content, promise) {
        var manga = JSON.parse(content);
        manga.externalId = externalId;
        manga.id = id;

        //we clean up all the information we don't need
        normalizeManga(manga);

        //store the manga in the DB
        updateDB(manga);

        logger.log('MangaEden - Manga %s updated', manga.title);
        promise.resolve(true);
    });
}

function updateChapter(id) {

}

function update() {
    var actions = [updateList, updateAllMangas];

    return promised.seq(actions);
}

module.exports.update = update;