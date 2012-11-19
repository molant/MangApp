/**
 * User: molant
 * Date: 11/16/12
 * Time: 10:35 PM
 */
"use strict";
var mongojs = require('mongojs'),
    db = mongojs('mangapp', ['mangas', 'chapters', 'mangaMap', 'updates']),
    ObjectId = mongojs.ObjectId,
    promised = require("promised-io/promise"),
    logger = require('tracer').console({
        format:"{{timestamp}} <{{title}}> {{message}} (in {{file}}:{{line}})",
        dateformat:"HH:MM:ss.L"
    }),
    Deferred = require('promised-io').Deferred,
    latestVersion = 1;

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

function getMangaIndex(externalId, providerId) {
    var deferred = new Deferred();
    db.mangaMap.find({externalId:externalId, providerId:providerId}, function (err, docs) {
        if (!err) {
            if (docs.length > 0) {
                deferred.resolve({
                    id:objectId,
                    new:false});
            } else {
                //TODO: check for the name of the manga here in case it is another provider
                //we create a new identifier
                var objectId = db.bson.ObjectID.createPk();

                db.mangaMap.save({_id:objectId, externalId:externalId, providerId:providerId}, function (err) {
                    if (!err) {
                        deferred.resolve({
                            id:objectId,
                            new:true
                        });
                    } else {
                        logger.log('Error updating mangaMap for %s', manga.title);
                        deferred.reject();
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

    db.mangas.save(manga, function (err) {
        if (!err) {
            deferred.resolve();
        } else {
            deferred.reject();
        }
    });

    return deferred.promise;
}

function insertChapter(chapter) {
    var deferred = new Deferred();
    chapter.version = latestVersion;
    db.chapters.save(chapter, function (err) {
        if (!err) {
            deferred.resolve(true);
        } else {
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

    getMangaIndex(manga, externalId, providerId)
        .then(function (index) {
            manga._id = index.id;
            manga.version = latestVersion;

            if (index.new) {
                insertManga(manga).then(function () {
                    deferred.resolve({status:true, chapters:true});
                });
            } else {
                updateManga(manga).then(function (updates) {
                    deferred.resolve(updates);
                });
            }
        });

    return deferred.promise;
}

//Manga already exists
function updateManga(manga) {
    var deferred = new Deferred();
    //update version
    getManga(manga._id).then(function (oldManga) {
        var result = {
            status:oldManga.status === manga.status,
            chapters:oldManga.chapter_len === manga.chapter_len
        };

        if (result.status || result.chapters) {
            //TODO: version should have a new value and not be incremented
            db.mangas.update({_id:manga._id}, {$set:{
                status:manga.status,
                chapters_len:manga.chapters_len,
                version:'new value here'
            }}, function (err) {
                if (!err) {
                    deferred.resolve(result);
                } else {
                    deferred.reject();
                }
            });
            //update
        } else {
            deferred.resolve(result);
        }
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
        if (!err) {
            deferred.resolve(docs);
        } else {
            deferred.reject();
        }
    });
    return deferred.promise;
}

function getChapters(id, limit) {
    var deferred = new Deferred();
    db.chapters.find({mangaId:id}, {_id:1, uploadedDate:1, number:1, title:1}, function (err, docs) {
        if (!err) {
            deferred.resolve(docs);
        } else {
            deferred.reject();
        }
    });
    return deferred.promise;
}

function getChapter(chapterId) {
    var deferred = new Deferred();
    db.chapters.find({_id:ObjectId(chapterId)}, {number:1, pages:1, title:1}, function (err, docs) {
        if (!err) {
            deferred.resolve(docs);
        } else {
            deferred.reject();
        }
    });

    return deferred.promise;
}

function addChapters(chapters) {
    var deferred = new Deferred();
    //TODO: insert the chapters!!
    deferred.resolve();
    return deferred.promise;
}

function startUpdate(){
    var deferred = new Deferred();
    db.updates.find({}).sort({version:1}).limit(1,function(err,docs){
        if(!err){
           if(docs.length > 0){
               latestVersion = docs[0].version++;
           }
            //UTC timestamp calculated using this link:
            //http://stackoverflow.com/questions/9756120/utc-timestamp-in-javascript

           db.updates.save({version:latestVersion, timestamp: Math.floor((new Date()).getTime() / 1000)},function(err){
              if(!err){
                  deferred.resolve();
              }else{
                  deferred.reject();
              }
           });
       }else{
            deferred.reject();
        }
    });

    return deferred.promise;
}

module.exports.clean = clean;
module.exports.addOrUpdateManga = addManga;
module.exports.getList = getList;
module.exports.getManga = getManga;
module.exports.getChapters = getChapters;
module.exports.getChapter = getChapter;
module.exports.addChapters = insertChapters;
module.exports.startUpdate = startUpdate;
//TODO: add methods to update individual items and to check if they already exist?

