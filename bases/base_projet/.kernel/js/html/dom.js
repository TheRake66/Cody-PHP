import Finder from './finder.js';
import Builder from './builder.js';



/**
 * Librairie gérant le DOM (Document Object Model).
 * 
 * @author Thibault Bustos (TheRake66)
 * @version 1.0.0.0
 * @category Framework source
 * @license MIT License
 * @copyright © 2021-2023 - Thibault BUSTOS (TheRake66)
 */
export default class Dom {

    /**
     * @type {string} Les positions possibles dans le DOM.
     */
	static POSITION_BEFORE_BEGIN = 'beforebegin';
	static POSITION_AFTER_BEGIN  = 'afterbegin';
	static POSITION_BEFORE_END   = 'beforeend';
	static POSITION_AFTER_END    = 'afterend';


    /**
     * Exécute une fonction si le DOM est chargé, sinon attends qu'il soit chargé pour l'exécuter.
     * 
     * @param {function} callback La fonction à exécuter.
     * @param {boolean} fully Si true, la fonction est exécutée quand le DOM est entièrement chargé (images, frames, etc...). Sinon, elle est exécutée quand le DOM est prêt mais pas entièrement chargé.
     * @returns {void}
     */
    static loaded(callback, fully = false) {
        if (fully) {
            if(document.readyState === "complete") {
                callback();
            } else {
                window.addEventListener("load", callback);
            }
        } else {
            if(document.readyState === "interactive") {
                callback();
            } else {
                window.addEventListener("DOMContentLoaded", callback);
            }
        }
    }
    

    /**
     * Vide le contenu d'un élément HTML.
     * 
     * @param {HTMLElement} el L'élément HTML.
     * @returns {void}
     */
    static clear(el = document.body) { 
        el.innerHTML = '';
    }
    

    /**
     * Détruit un élément HTML.
     * 
     * @param {HTMLElement} el L'élément HTML.
     * @returns {void}
     */
    static destroy(el = document.body) {
        el.remove();
    }


    /**
     * Insert du code HTML dans un élément HTML a une position donnée.
     * 
     * @param {string} html Le contenu HTML.
     * @param {HTMLElement} el L'élément HTML.
     * @param {string} position L'endroit où insérer le contenu HTML.
     * @returns {void}
     */
    static insert(html, el = document.body, position = Dom.POSITION_BEFORE_END) {
        el.insertAdjacentHTML(position, html);
    }


    /**
     * Vide le contenu d'un élément HTML puis insert nouvel élément ou code HTML.
     * 
     * @param {HTMLElement|string} content L'élément HTML ou le code HTML.
     * @param {HTMLElement} el L'élément HTML.
     * @returns {void}
     */
    static replace(content, el = document.body) {
        if (content instanceof HTMLElement) {
            el.innerHTML = '';
            el.appendChild(content);
        } else {
            el.innerHTML = content;
        }
    }


    /**
     * Insert un élément HTML dans le DOM.
     * 
     * @param {HTMLElement} el L'élément HTML à insérer.
     * @param {HTMLElement} parent Le parent de l'élément HTML.
     * @returns {void}
     */
    static append(el, parent = document.body) {
        parent.appendChild(el);
    }


    /**
     * Insert un élément HTML dans le DOM après un autre élément HTML.
     * 
     * @param {HTMLElement} el L'élément HTML à insérer.
     * @param {HTMLElement} sibling L'élément HTML après lequel insérer l'élément.
     * @returns {void}
     */
    static after(el, sibling) {
        sibling.parentNode.insertBefore(el, sibling.nextSibling);
    }


    /**
     * Insert un élément HTML dans le DOM avant un autre élément HTML.
     * 
     * @param {HTMLElement} el L'élément HTML à insérer.
     * @param {HTMLElement} sibling L'élément HTML avant lequel insérer l'élément.
     */
    static before(el, sibling) {
        sibling.parentNode.insertBefore(el, sibling);
    }


    /**
     * Insert un élément HTML dans le DOM en premier.
     * 
     * @param {HTMLElement} el L'élément HTML à insérer.
     * @param {HTMLElement} parent Le parent de l'élément HTML.
     * @returns {void}
     */
    static prepend(el, parent = document.body) {
        parent.insertBefore(el, parent.firstChild);
    }


    /**
     * Supprime un élément HTML dans le DOM.
     * 
     * @param {HTMLElement} el L'élément HTML à supprimer.
     * @param {HTMLElement} parent Le parent de l'élément HTML.
     * @returns {void}
     */
    static remove(el, parent = document.body) {
        parent.removeChild(el);
    }


    /**
     * Insert un composant dans le DOM via son code HTML.
     * 
     * @param {string} component Le code HTML du composant.
     * @param {HTMLElement} parent Le parent du composant.
     * @param {string} position L'endroit où insérer le composant.
     * @returns {void}
     */
    static inject(component, parent = document.body, position = Dom.POSITION_BEFORE_END) {
        let element = Builder.parse(component);
        
        switch (position) {
            case 'beforebegin':
                Dom.before(element, parent);
                break;

            case 'afterbegin':
                Dom.prepend(element, parent);
                break;

            case 'beforeend':
                Dom.append(element, parent);
                break;

            case 'afterend':
                Dom.after(element, parent);
                break;

            default:
                Dom.append(element, parent);
                break;
        }

        let less = window.less;
        if (less !== undefined) {
            less.registerStylesheets();
            less.refresh();
        }
        
        let scripts = Finder.queryAll('script', element);
        if (scripts) {
            scripts.forEach(script => {
                let html = script.innerHTML;
                let type = script.type;
                let container = script.parentNode;
                let newScript = Builder.create('script', { 
                    type: type
                }, html);
                container.replaceChild(newScript, script);
            });
        }
    }

}