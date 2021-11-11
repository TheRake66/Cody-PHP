<?php

namespace Librairie;



class Security {

    /**
     * Genere une chaine aleatoire de taille n
     * 
     * @param int taille de la chaine
     * @return string chaine aleatoire
     */
	public static function genererRandom($nbLetters) {
		$randString = '';
		$charUniverse = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789\\=';
		for($i = 0; $i < $nbLetters; $i++) {
		   $randString .= $charUniverse[rand(0, strlen($charUniverse) - 1)];
		}
		return $randString;
	}


	/**
	 * Verifie et active le protocole SSL
	 */
	public static function activerSSL() {
		if($_SERVER['SERVER_PORT'] !== 443 &&
			(empty($_SERVER['HTTPS']) || $_SERVER['HTTPS'] === 'off')) {
			header('Location: https://' . $_SERVER['HTTP_HOST'] . $_SERVER['REQUEST_URI']);
			exit;
		}
	}
	
}

?>