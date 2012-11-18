/**
 * User: molant
 * Date: 11/16/12
 * Time: 10:35 PM
 */
"use strict";
var mongojs = require('mongojs'),
    db = mongojs('mangapp', ['mangas', 'chapters', 'mangaMap', 'chapterMap']),
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
        db.mangas.remove({});
        db.chapters.remove({});
        db.mangaMap.remove({});
        db.chapterMap.remove({});
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

function updateManga(manga) {
    var deferred = new Deferred(),
        id = manga._id;

    //if we are going to do an update we cannot update the ._id
    //delete manga._id;

    db.mangas.save(manga, function (err) {
        if (!err) {
            deferred.resolve(true);
        } else {
            deferred.resolve(false);
        }
    });

    //TODO: here we should check what has changed or not and update accordingly (+ adding a timestamp or something)
    /*db.mangas.update({_id:manga._id}, {$set:manga}, {upsert:true}, function (err) {
     if (!err) {
     deferred.resolve({
     id:manga._id,
     status:true
     });
     } else {
     logger.log('Error inserting %s', manga.title);
     deferred.resolve({
     id:manga._id,
     status:false
     });
     }
     });*/

    return deferred.promise;
}

function addManga(manga, externalId, providerId) {
    var deferred = new Deferred();

    getMangaIndex(manga, externalId, providerId)
        .then(updateManga).then(function (result) {
            logger.log('Manga %s %s', manga.title, result ? 'updated' : 'not updated');
            deferred.resolve(result);
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


module.exports.clean = clean;
module.exports.addManga = addManga;
module.exports.getList = getList;
