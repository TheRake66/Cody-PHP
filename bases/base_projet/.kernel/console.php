<?php
require_once(__DIR__ . '/php/io/autoloader.php');
use Kernel as k;
use Cody as c;



// Enregistre l'autoloader de classe.
k\Io\Autoloader::register();

// Charge la configuration.
k\Environnement\Configuration::load();

// Supprime l'écouteur d'événement des erreurs.
//k\Debug\Error::remove();

// Désafiche le journal de log.
k\Debug\Log::disable();

// Lance la console
c\Console\Program::main();

?>