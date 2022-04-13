import Http from './http.js';
import Url from './url.js';



/**
 * Librairie de communication avec l'API REST en PHP
 */
export default class Rest {
    
    /**
     * Execute une requete REST de type GET puis retourne le resultat
     * 
     * @param {string} route la route
     * @param {string} rest le nom de la fonction cote API
     * @param {function} sucess fonction anonyme appeler lors de la reponse
     * @param {function} empty fonction anonyme appeler si resultat vide
     * @param {function} failed fonction anonyme appeler si echec
     * @param {function} expired fonction anonyme appeler si temps d'attente depasse
     * @param {Array} param les parametres supplementaires a l'URL
     * @param {Number} timeout le temps d'attente avant echec
     * @returns void
     */
    static get(route, rest, sucess = null, empty = null, failed = null, expired = null, param = {}, timeout = null) {
        return Rest.#ask(route, rest, sucess, empty, failed, expired, param, Http.METHOD_GET, timeout);
    }
    

    /**
     * Execute une requete REST de type POST puis retourne le resultat
     * 
     * @param {string} route la route
     * @param {string} rest le nom de la fonction cote API
     * @param {function} sucess fonction anonyme appeler lors de la reponse
     * @param {function} empty fonction anonyme appeler si resultat vide
     * @param {function} failed fonction anonyme appeler si echec
     * @param {function} expired fonction anonyme appeler si temps d'attente depasse
     * @param {Array} param les parametres supplementaires dans le corps de la requete
     * @param {Number} timeout le temps d'attente avant echec
     * @returns void
     */
    static post(route, rest, sucess = null, empty = null, failed = null, expired = null, param = {}, timeout = null) {
        return Rest.#ask(route, rest, sucess, empty, failed, expired, param, Http.METHOD_POST, timeout);
    }

    
    /**
     * Execute une requete REST de type GET puis boucle sur les resultats
     * 
     * @param {string} route la route
     * @param {string} rest le nom de la fonction cote API
     * @param {function} sucess fonction anonyme appeler sur chaque reponse
     * @param {function} pre fonction anonyme appeler avant l'iteration
     * @param {function} post fonction anonyme appeler apres l'iteration
     * @param {function} empty fonction anonyme appeler si resultat vide
     * @param {function} failed fonction anonyme appeler si echec
     * @param {function} expired fonction anonyme appeler si temps d'attente depasse
     * @param {Array} param les parametres supplementaires a l'URL
     * @param {Number} timeout le temps d'attente avant echec
     * @returns void
     */
    static getFor(route, rest, sucess = null, pre = null, post = null, empty = null, failed = null, expired = null, param = {}, timeout = null) {
        return Rest.#askFor(route, rest, sucess, pre, post, empty, failed, expired, param, Http.METHOD_GET, timeout = null);
    }

    
    /**
     * Execute une requete REST de type POST puis boucle sur les resultats
     * 
     * @param {string} route la route
     * @param {string} rest le nom de la fonction cote API
     * @param {function} sucess fonction anonyme appeler sur chaque reponse
     * @param {function} pre fonction anonyme appeler avant l'iteration
     * @param {function} post fonction anonyme appeler apres l'iteration
     * @param {function} empty fonction anonyme appeler si resultat vide
     * @param {function} failed fonction anonyme appeler si echec
     * @param {function} expired fonction anonyme appeler si temps d'attente depasse
     * @param {Array} param les parametres supplementaires dans le corps de la requete
     * @param {Number} timeout le temps d'attente avant echec
     * @returns void
     */
    static postFor(route, rest, sucess = null, pre = null, post = null, empty = null, failed = null, expired = null, param = {}, timeout = null) {
        return Rest.#askFor(route, rest, sucess, pre, post, empty, failed, expired, param, Http.METHOD_POST, timeout);
    }


    /**
     * Execute une requete REST puis retourne le resultat
     * 
     * @param {string} route la route
     * @param {string} rest le nom de la fonction cote API
     * @param {function} sucess fonction anonyme appeler lors de la reponse
     * @param {function} empty fonction anonyme appeler si resultat vide
     * @param {function} failed fonction anonyme appeler si echec
     * @param {function} expired fonction anonyme appeler si temps d'attente depasse
     * @param {Array} param les parametres supplementaires a l'URL
     * @param {string} method la methode d'envoi
     * @param {Number} timeout le temps d'attente avant echec
     * @returns void
     */
    static #ask(route, rest, sucess = null, empty = null, failed = null, expired = null, param = {}, method = Http.METHOD_GET, timeout = null) {
        let _ = {};
        _['routePage'] = route;
        _['restFunction'] = rest;
        param = Object.assign({}, _, param);

        Http.send(
            Url.root(),
            response => {
                if (response !== '') {
                    let json = null;
                    let continu = true;
                    try {
                        json = JSON.parse(response);
                    } catch (error) {
                        continu = false;
                    }
                    if (continu) {
                        if (json !== null) {
                            if (sucess) sucess(json);
                        } else {
                            if (empty) empty();
                        }
                    } else {
                        if (failed) failed();
                    }
                } else {
                    if (empty) empty();
                }
            },
            failed,
            expired,
            method,
            param,
            timeout
        );
    }


    /**
     * Execute une requete REST puis boucle sur les resultats
     * 
     * @param {string} route la route
     * @param {string} rest le nom de la fonction cote API
     * @param {function} sucess fonction anonyme appeler sur chaque reponse
     * @param {function} pre fonction anonyme appeler avant l'iteration
     * @param {function} post fonction anonyme appeler apres l'iteration
     * @param {function} empty fonction anonyme appeler si resultat vide
     * @param {function} failed fonction anonyme appeler si echec
     * @param {function} expired fonction anonyme appeler si temps d'attente depasse
     * @param {Array} param les parametres supplementaires a l'URL
     * @param {string} method la methode d'envoi
     * @param {Number} timeout le temps d'attente avant echec
     * @returns void
     */
    static #askFor(route, rest, sucess = null, pre = null, post = null, empty = null, failed = null, expired = null, param = {}, method = Http.METHOD_GET, timeout = null) {
        let _ = {};
        _['routePage'] = route;
        _['restFunction'] = rest;
        param = Object.assign({}, _, param);
        
        Http.send(
            Url.root(),
            response => {
                if (response !== '') {
                    let json = null;
                    let continu = true;
                    try {
                        json = JSON.parse(response);
                    } catch (error) {
                        continu = false;
                    }
                    if (continu) {
                        if (json !== null && json.length > 0) {
                            if (pre) pre();
                            json.forEach(element => sucess(element));
                            if (post) post();
                        } else {
                            if (empty) empty();
                        }
                    } else {
                        if (failed) failed();
                    }
                } else {
                    if (empty) empty();
                }
            },
            failed,
            expired,
            method,
            param,
            timeout
        );
    }

}