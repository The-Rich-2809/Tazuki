-- -----------------------------------------------------
-- Script para inicializar la base de datos (v2, con tabla de tags)
-- -----------------------------------------------------

-- 1. Crear la base de datos si no existe
CREATE DATABASE IF NOT EXISTS `disenos_db` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;

-- 2. Usar la base de datos
USE `disenos_db`;

-- 3. Borrar tablas si existen para empezar de cero
DROP TABLE IF EXISTS `diseno_tags`;
DROP TABLE IF EXISTS `tags`;
DROP TABLE IF EXISTS `disenos`;

-- 4. Crear la tabla `disenos` (sin la columna de tags)
CREATE TABLE `disenos` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(255) NOT NULL,
  `descripcion` TEXT NULL,
  `ruta_imagen` VARCHAR(512) NULL,
  `fecha_creacion` DATE NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE = InnoDB;

-- 5. Crear la tabla `tags`
CREATE TABLE `tags` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(100) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `nombre_UNIQUE` (`nombre` ASC)
) ENGINE = InnoDB;

-- 6. Crear la tabla de unión `diseno_tags`
CREATE TABLE `diseno_tags` (
  `diseno_id` INT NOT NULL,
  `tag_id` INT NOT NULL,
  PRIMARY KEY (`diseno_id`, `tag_id`),
  INDEX `fk_diseno_tags_tags_idx` (`tag_id` ASC),
  CONSTRAINT `fk_diseno_tags_disenos`
    FOREIGN KEY (`diseno_id`)
    REFERENCES `disenos` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_diseno_tags_tags`
    FOREIGN KEY (`tag_id`)
    REFERENCES `tags` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE
) ENGINE = InnoDB;

-- 7. Insertar datos de ejemplo
-- Insertar diseños (IDs 1 y 2)
INSERT INTO `disenos` (`id`, `nombre`, `descripcion`, `ruta_imagen`, `fecha_creacion`) VALUES 
(1, 'Gato Programador', 'Un gato con lentes escribiendo código en una laptop.', 'C:\Users\Rich\Documents\Gemini\disenos\gato_programador.png', '2025-09-25'),
(2, 'Flores de Acuarela', 'Un patrón floral delicado pintado en acuarela.', 'C:\Users\Rich\Documents\Gemini\disenos\flores_acuarela.jpg', '2025-09-24');

-- Insertar etiquetas (IDs 1 al 8)
INSERT INTO `tags` (`id`, `nombre`) VALUES
(1, 'gato'), (2, 'programacion'), (3, 'gracioso'), (4, 'animales'),
(5, 'flores'), (6, 'acuarela'), (7, 'elegante'), (8, 'naturaleza');

-- Vincular diseños con etiquetas
INSERT INTO `diseno_tags` (`diseno_id`, `tag_id`) VALUES
-- Gato Programador (ID 1) tiene tags: gato(1), programacion(2), gracioso(3), animales(4)
(1, 1), (1, 2), (1, 3), (1, 4),
-- Flores de Acuarela (ID 2) tiene tags: flores(5), acuarela(6), elegante(7), naturaleza(8)
(2, 5), (2, 6), (2, 7), (2, 8);

-- --- Fin del script ---