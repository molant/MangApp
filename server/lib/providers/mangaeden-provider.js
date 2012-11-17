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

function normalizeManga(manga) {
    delete manga['baka'];
    delete manga['language'];
    delete manga['aka-alias'];
    delete manga['aka'];
    delete manga['type'];
    delete manga.imageURL;
    delete manga.chapters;
    manga.image = 'http://cdn.mangaeden.com/mangasimg/' + manga.image;
}

function updateManga(externalId, index) {
    var uri = 'http://www.mangaeden.com/api/manga/' + externalId;

    return requestParser(uri, function (content, promise) {
        var manga = JSON.parse(content);
        //we clean up all the information we don't need
        normalizeManga(manga);
        mangaDb.addManga(manga, externalId, providerId).then(function (result) {
            promise.resolve(result);
        });
    });
}

function updateChapter(id) {

}

function update() {
    var actions = [updateList, updateAllMangas];

    return promised.seq(actions);
}

module.exports.update = update;