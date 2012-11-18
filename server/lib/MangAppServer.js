/*
 * MangAppServer
 * https://github.com/molant/MangApp
 *
 * Copyright (c) 2012 molant
 * Licensed under the MIT license.
 */
"use strict";
var restify = require('restify'),
    providers = require('./providers/provider-loader.js').providers,
    promised = require("promised-io/promise"),
    mangaDb = require('./helpers/database.js'),
    logger = require('tracer').console({
        format:"{{timestamp}} <{{title}}> {{message}}",
        dateformat:"HH:MM:ss.L"
    });

var server = restify.createServer({
    name:'MangApp'
});

server.get({path:'/list/', version:'1.0.0'}, list);
function list(req, res, next) {
    logger.log('List petition');
    mangaDb.getList().then(function (docs) {
        res.contentType = 'json';
        res.send(docs);
    }, function (err) {
        console.log(err);
    });
}

server.get({path:'/list/update/', version:'1.0.0'}, updateDB);
function updateDB(req, res, next) {
    var updaters = [];
    logger.log('updating DB');
    for (var i = 0; i < providers.length; i++) {
        updaters.push(providers[i].update());
    }

    promised.all(updaters).then(function (array) {
        res.send(array.length + ' servers updated');
        logger.log('DB updated');
    });
}

server.get({path:'/list/clean/', version:'1.0.0'}, cleanDB);
function cleanDB(req, res, next) {
    var cleaned = mangaDb.clean();
    res.send('Database clean: ' + cleaned);
}

server.get({path:'/manga/:id', version:'1.0.0'}, manga);
function manga(req, res, next) {
    logger.log('Manga requested - %s', req.params.id);
    mangaDb.getManga(req.params.id).then(function (manga) {
        mangaDb.getChapters(req.params.id).then(function (chapters) {
            manga.chapters = chapters;
            res.contentType = 'json';
            res.send(manga);
        });
    });
}


server.get({path:'/version/', version:'1.0.0'}, function (req, res, next) {
    res.send('1.0.0');
});

server.get({path:'/manga/:id/:chapterId', version:'1.0.0'}, chapter);
function chapter(req, res, next) {
    mangaDb.getChapter(req.params.chapterId).then(function (chapters) {
        res.contentType = 'json';
        res.send(chapters);
    });
    console.log(req.params);
}

server.listen(32810);


