const ReqType = {
	GET: "GET",
	POST: "POST"
}



class HTTP {

    /**
     * Constructeur
     */
    constructor() {
    }


    /**
     * Execute une requete http(s) en async
     * 
     * @param {string} url - URL a requeter
     * @param {function} callback - Fonction anonyme appeler lors de la reponse
     * @param {ReqType} type - Type de requete
     */
    send(url, callback, type = ReqType.GET) {
        let xml = new XMLHttpRequest();
        xml.open(type, url, true);
        xml.onreadystatechange = () => {
            if (xml.status == 200 && xml.readyState == 4)
                callback(xml.response);
        }
        xml.send();
    };

};