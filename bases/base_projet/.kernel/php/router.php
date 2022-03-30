<?php
namespace Kernel;



/**
 * Librairie gerant le routage des pages
 */
class Router {

    /**
     * Liste des routes
	 * 
	 * @var array
     */
	private static $routes = [];

    /**
     * Route par defaut
	 * 
	 * @var string
     */
	private static $default;

    /**
     * Route si non trouve
	 * 
	 * @var string
     */
	private static $notfound;

    /**
     * Route actuelle
	 * 
	 * @var string
     */
	private static $current;


    /**
     * Configure la route par defaut
	 * 
	 * @param string nom de la route
	 * @return void
     */
	static function default($defaut) {
		self::$default = $defaut;
	}


	/**
	 * Charge les routes
	 * 
	 * @return void
	 * @throws Si le fichier de route n'est pas trouvé
	 */
	static function load() {
		$f = 'debug/app/route.php';
		if (is_file($f) && is_readable($f)) {
			include $f;
		} else {
			trigger_error('Impossible de charger les routes, le fichier "' . $f . '" est introuvable !');
		}
	}


    /**
     * Configure la route en cas de route non trouvee (404)
	 * 
	 * @param string nom de la route
	 * @return void
     */
	static function notfound($notfound) {
		self::$notfound = $notfound;
	}


    /**
     * Ajoute une route
	 * 
	 * @param string nom de la route
	 * @param object classe du controleur
	 * @return void
     */
	static function add($nom, $route) {
		self::$routes[$nom] = $route;
	}
	

	/**
	 * Retourne la route actuelle
	 * 
	 * @return string le nom de la route
	 * @throws Si aucune route n'a ete definie
	 */
	static function get() {
		if (is_null(self::$current)) {
			$r = null;
			foreach ([ $_GET, $_POST, $_SESSION ] as $a) {
				if (isset($a['routePage'])) {
					if (self::exist($a['routePage'])) {
						$r = $a['routePage'];
					} elseif(self::exist(self::$notfound)) {
						http_response_code(404);
						$r = self::$notfound;
					}
					break;
				}
			}
			if (is_null($r)) {
				if(self::exist(self::$default)) {
					$r = self::$default;
				} else {
					$r = self::getFirst();
					if (is_null($r)) {
						trigger_error("Aucune route n'a été définie.");
					}
				}
			}
			self::$current = $r;
			return $r;
		} else {
			return self::$current;
		}
	}
	

	/**
	 * Retourne le controleur actuel
	 * 
	 * @return object le controleur
	 */
	static function getController() {
		return self::$routes[self::get()];
	}


	/**
	 * Retourne la premiere route
	 * 
	 * @return string nom de la premiere route
	 */
	static function getFirst() {
		if (count(self::$routes) > 0) {
			return array_key_first(self::$routes);
		}
	}


	/**
	 * Verifi si une route existe
	 * 
	 * @param string nom de la route
	 * @return bool si existe
	 */
	static function exist($name) {
		return (!is_null($name) && array_key_exists($name, self::$routes));
	}


    /**
     * Appel la bonne route
	 * 
	 * @return void
     */
	static function routing() {
        Debug::log('Routage (url : "' . Url::current() . '")...', Debug::LEVEL_PROGRESS);

		$c = self::getController();
        Debug::log('Contrôleur identifié : "' . $c . '".');

		new $c();
		Debug::log('Routage fait.', Debug::LEVEL_GOOD);
	}
	
}

?>