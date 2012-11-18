/**
 * User: molant
 * Date: 11/16/12
 * Time: 10:35 PM
 */
"use strict";
var mongojs = require('mongojs'),
    db = mongojs('mangapp', ['mangas', 'chapters', 'mangaMap']),
    ObjectId = mongojs.ObjectId,
    promised = require("promised-io/promise"),
    logger = require('tracer').console({
        format:"{{timestamp}} <{{title}}> {{message}} (in {{file}}:{{line}})",
        dateformat:"HH:MM:ss.L"
    }),
    Deferred = require('promised-io').Deferred;

process.setMaxListeners(0);


function clean() {
    try {
        logger.log('Cleaning DB');
        db.mangas.remove({});
        db.chapters.remove({});
        db.mangaMap.remove({});
        return true;
    } catch (exc) {
        return false;
    }
}

function getMangaIndex(manga, externalId, providerId) {
    var deferred = new Deferred();
    db.mangaMap.find({externalId:externalId, providerId:providerId}, function (err, docs) {
        if (!err) {
            if (docs.length > 0) {
                deferred.resolve(docs[0]._id);
            } else {
                //TODO: check for the name of the manga here in case it is another provider
                //we create a new identifier
                var objectId = db.bson.ObjectID.createPk();

                db.mangaMap.save({_id:objectId, externalId:externalId, providerId:providerId}, function (err) {
                    if (err) {
                        logger.log('Error updating mangaMap for %s', manga.title);
                        deferred.reject();
                    } else {
                        manga._id = objectId;
                        deferred.resolve(manga);
                    }
                });
            }
        } else {
            deferred.reject();
        }
    });

    return deferred.promise;
}

function insertManga(manga) {
    var deferred = new Deferred();

    //if we are going to do an update we cannot update the ._id
    //delete manga._id;

    db.mangas.save(manga, function (err) {
        if (!err) {
            deferred.resolve(true);
        } else {
            deferred.resolve(false);
        }
    });

    return deferred.promise;
}

function insertChapter(chapter) {
    var deferred = new Deferred();
    db.chapters.save(chapter,function(err){
        if(!err){
            deferred.resolve(true);
        }else{
            deferred.resolve(false);
        }
    });

    return deferred.promise;
}

function insertChapters(chapters, mangaId, providerId) {
    var deferred = new Deferred(),
        i = -1;

    function next() {
        i++;
        if (i < chapters.length) {
            chapters[i].mangaId = mangaId.toString();
            chapters[i].providerId = providerId;
            insertChapter(chapters[i])
                .then(next);
        } else {
            deferred.resolve();
        }
    }
    next();

    return deferred.promise;
}

function addManga(manga, externalId, providerId) {
    var deferred = new Deferred(),
        chapters = manga.chapters;
    delete manga.chapters;

//we clean up all the information we don't need
    getMangaIndex(manga, externalId, providerId)
        .then(insertManga).then(function (result) {
            insertChapters(chapters, manga._id, providerId).then(function () {
                logger.log('Manga %s %s', manga.title, result ? 'updated' : 'not updated');
                deferred.resolve(result);
            });
        });

    return deferred.promise;
}

function getList(diff) {
    var deferred = new Deferred();
    db.mangas.find(function (err, docs) {
        if (!err) {
            deferred.resolve(docs);
        } else {
            deferred.reject();
        }
    });

    return deferred.promise;
};


function getManga(id) {
    var deferred = new Deferred();
    db.mangas.findOne({_id:ObjectId(id)}, function (err, docs) {
        deferred.resolve(docs);
    });
    return deferred.promise;
}

function getChapters(id, limit) {
    var deferred = new Deferred();
    db.chapters.find({mangaId:id},{_id:1,uploadedDate:1,number:1,title:1}, function (err, docs) {
        deferred.resolve(docs);
    });
    return deferred.promise;
}

function getChapter(chapterId){
    var deferred = new Deferred();
    db.chapters.find({_id: ObjectId(chapterId)},{number:1,pages:1,title:1},function(err,docs){
        deferred.resolve(docs);
    });

    return deferred.promise;
}


module.exports.clean = clean;
module.exports.addManga = addManga;
module.exports.getList = getList;
module.exports.getManga = getManga;
module.exports.getChapters = getChapters;
module.exports.getChapter = getChapter;
//TODO: add methods to update individual items and to check if they already exist?

