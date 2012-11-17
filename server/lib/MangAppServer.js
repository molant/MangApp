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
    }),
    Deferred = require('promised-io').Deferred;

var server = restify.createServer({
    name:'MangApp'
});

//server.use(restify.bodyParser());

server.get({path:'/list/', version:'1.0.0'}, list);
server.get({path:'/list/update/', version:'1.0.0'}, updateDB);
server.get({path:'/list/clean/', version:'1.0.0'}, cleanDB);
server.get({path:'/version/', version:'1.0.0'}, function(req,res,next){
    res.send('1.0.0');
});
server.get({path:'/list/clean/update/', version:'1.0.0'}, function(req,res,next){
    mangaDb.clean();
    updateDB(req,res,next);
});
server.get({path:'/update/:id', version:'1.0.0'}, update);
server.get({path:'/manga/:id', version:'1.0.0'}, manga);
server.get({path:'/manga/:id/:chapterId', version:'1.0.0'}, chapter);
/*server.get({path:'/list/', version:'1.0.0'}, list);
 server.get({path:'/list/', version:'1.0.0'}, list);*/
server.listen(32810);

function list(req, res, next) {
    var mangas = db.getList();
    res.send(JSON.stringify(mangas));
}

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

function cleanDB(req, res, next) {
    var cleaned = mangaDb.clean();
    res.send('Database clean: ' + cleaned);
}

function update(req, res, next) {
    //check for mangas updated since the requested version
}

function manga(req, res, next) {
    console.log(req.params);
}

function chapter(req, res, next) {
    console.log(req.params);
}