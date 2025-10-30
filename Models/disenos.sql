CREATE DATABASE IF NOT EXISTS disenos_db DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;

USE disenos_db;

-- Se eliminan las tablas en orden inverso para evitar problemas de FK
DROP TABLE IF EXISTS diseno_tags;
DROP TABLE IF EXISTS disenos;
DROP TABLE IF EXISTS tags;
DROP TABLE IF EXISTS tamanos_taza;
DROP TABLE IF EXISTS direcciones;
DROP TABLE IF EXISTS usuarios;
DROP TABLE IF EXISTS carrito_items;

-- -----------------------------------------------------
-- Tabla `carrito_items`
-- -----------------------------------------------------
CREATE TABLE carrito_items (
  usuario_id INT NOT NULL,
  diseno_id INT NOT NULL,
  tamano_taza_id INT NOT NULL, -- Columna de tamaño de taza solicitada
  cantidad INT NOT NULL DEFAULT 1,
  fecha_agregado DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  
  -- Clave primaria compuesta para un item único por usuario, diseño y tamaño
  PRIMARY KEY (usuario_id, diseno_id, tamano_taza_id), 
  
  INDEX fk_carrito_items_disenos_idx (diseno_id ASC),
  INDEX fk_carrito_items_tamanos_taza_idx (tamano_taza_id ASC),
  
  CONSTRAINT fk_carrito_items_usuarios
    FOREIGN KEY (usuario_id)
    REFERENCES usuarios (id)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT fk_carrito_items_disenos
    FOREIGN KEY (diseno_id)
    REFERENCES disenos (id)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT fk_carrito_items_tamanos_taza
    FOREIGN KEY (tamano_taza_id)
    REFERENCES tamanos_taza (id)
    ON DELETE RESTRICT -- Evita borrar un tamaño si está en un carrito
    ON UPDATE CASCADE
) ENGINE = InnoDB;
-- -----------------------------------------------------
-- Tabla `tamanos_taza`
-- -----------------------------------------------------
CREATE TABLE tamanos_taza (
  id INT NOT NULL AUTO_INCREMENT,
  nombre VARCHAR(255) NOT NULL,
  precio DECIMAL(10, 2) NOT NULL,
  PRIMARY KEY (id),
  UNIQUE INDEX nombre_UNIQUE (nombre ASC)
) ENGINE = InnoDB;

-- -----------------------------------------------------
-- Tabla `usuarios`
-- -----------------------------------------------------
CREATE TABLE usuarios (
  id INT NOT NULL AUTO_INCREMENT,
  nombre VARCHAR(100) NOT NULL,
  pri_apellido VARCHAR(100) NOT NULL,
  seg_apellido VARCHAR(100) NOT NULL,
  email VARCHAR(255) NOT NULL,
  password VARCHAR(255) NOT NULL, -- Recuerda hashear la contraseña
  rol ENUM('admin', 'usuario') NOT NULL DEFAULT 'usuario',
  fecha_registro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  UNIQUE INDEX email_UNIQUE (email ASC)
) ENGINE = InnoDB;

-- -----------------------------------------------------
-- Tabla `disenos` (MODIFICADA)
-- Esta es la versión original, sin relación con 'usuarios'.
-- -----------------------------------------------------
CREATE TABLE disenos (
  id INT NOT NULL AUTO_INCREMENT,
  nombre VARCHAR(255) NOT NULL,
  precio DECIMAL(10, 2) NOT NULL,
  tamano_taza_id INT NOT NULL,
  descripcion TEXT NULL,
  ruta_diseno VARCHAR(512) NULL,
  fecha_creacion DATE NOT NULL,
  publicado BOOLEAN NOT NULL DEFAULT FALSE,
  PRIMARY KEY (id),
  UNIQUE INDEX ruta_diseno_UNIQUE (ruta_diseno ASC),
  INDEX fk_disenos_tamanos_taza_idx (tamano_taza_id ASC),
  CONSTRAINT fk_disenos_tamanos_taza
    FOREIGN KEY (tamano_taza_id)
    REFERENCES tamanos_taza (id)
    ON DELETE RESTRICT
    ON UPDATE CASCADE
) ENGINE = InnoDB;

-- -----------------------------------------------------
-- Tabla `tags`
-- -----------------------------------------------------
CREATE TABLE tags (
  id INT NOT NULL AUTO_INCREMENT,
  nombre VARCHAR(100) NOT NULL,
  PRIMARY KEY (id),
  UNIQUE INDEX nombre_UNIQUE (nombre ASC)
) ENGINE = InnoDB;

-- -----------------------------------------------------
-- Tabla `diseno_tags`
-- -----------------------------------------------------
CREATE TABLE diseno_tags (
  diseno_id INT NOT NULL,
  tag_id INT NOT NULL,
  PRIMARY KEY (diseno_id, tag_id),
  INDEX fk_diseno_tags_tags_idx (tag_id ASC),
  CONSTRAINT fk_diseno_tags_disenos
    FOREIGN KEY (diseno_id)
    REFERENCES disenos (id)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT fk_diseno_tags_tags
    FOREIGN KEY (tag_id)
    REFERENCES tags (id)
    ON DELETE CASCADE
    ON UPDATE CASCADE
) ENGINE = InnoDB;

-- -----------------------------------------------------
-- Tabla `direcciones`
-- -----------------------------------------------------
CREATE TABLE direcciones (
  id INT NOT NULL AUTO_INCREMENT,
  usuario_id INT NOT NULL, -- El usuario al que pertenece la dirección
  calle VARCHAR(255) NOT NULL,
  numero_exterior VARCHAR(50) NOT NULL,
  numero_interior VARCHAR(50) NULL,
  colonia VARCHAR(100) NOT NULL,
  codigo_postal VARCHAR(10) NOT NULL,
  ciudad VARCHAR(100) NOT NULL,
  estado VARCHAR(100) NOT NULL,
  pais VARCHAR(100) NOT NULL DEFAULT 'México',
  referencias TEXT NULL,
  es_predeterminada BOOLEAN NOT NULL DEFAULT FALSE,
  PRIMARY KEY (id),
  INDEX fk_direcciones_usuarios_idx (usuario_id ASC),
  CONSTRAINT fk_direcciones_usuarios
    FOREIGN KEY (usuario_id)
    REFERENCES usuarios (id)
    ON DELETE CASCADE -- Si se borra el usuario, se borran sus direcciones
    ON UPDATE CASCADE
) ENGINE = InnoDB;