/*
 * MangAppServer
 * https://github.com/molant/MangApp
 *
 * Copyright (c) 2012 molant
 * Licensed under the MIT license.
 */
"use strict";
var restify = require('restify'),
    providers = require('./providers/provider-loader.js'),
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
    logger.log('updating DB');

    providers.update().then(function (array) {
        res.send(array.length + ' servers updated');
        logger.log('DB updated');
    },function(err){
        res.send('Error uploading the server');
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
        },function(err){
            res.send('Manga ' + req.params.id + ' not found');
        });
    },function(err){
        res.send('Manga ' + req.params.id + ' not found');
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
    },function(err){
        res.send('Chapter ' + req.params.id + ' not found');
    });
    console.log(req.params);
}

server.listen(32810);