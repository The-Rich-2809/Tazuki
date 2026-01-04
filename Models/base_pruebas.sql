CREATE DATABASE IF NOT EXISTS disenos_db_prueba DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE disenos_db_prueba;

-- -----------------------------------------------------
-- Limpieza de tablas (Orden inverso por FK)
-- -----------------------------------------------------
DROP TABLE IF EXISTS pedido_items;
DROP TABLE IF EXISTS pedidos;
DROP TABLE IF EXISTS carrito_items;
DROP TABLE IF EXISTS direcciones;
DROP TABLE IF EXISTS diseno_tags;
DROP TABLE IF EXISTS tags;
DROP TABLE IF EXISTS disenos;
DROP TABLE IF EXISTS usuarios;
DROP TABLE IF EXISTS tamanos_taza;

-- -----------------------------------------------------
-- 1. Catálogo de Tamaños de Taza
-- -----------------------------------------------------
CREATE TABLE tamanos_taza (
  id INT NOT NULL AUTO_INCREMENT,
  nombre VARCHAR(255) NOT NULL,
  precio DECIMAL(10, 2) NOT NULL,
  PRIMARY KEY (id),
  UNIQUE INDEX nombre_UNIQUE (nombre ASC)
) ENGINE = InnoDB;

-- -----------------------------------------------------
-- 2. Usuarios
-- -----------------------------------------------------
CREATE TABLE usuarios (
  id INT NOT NULL AUTO_INCREMENT,
  nombre VARCHAR(100) NOT NULL,
  pri_apellido VARCHAR(100) NOT NULL,
  seg_apellido VARCHAR(100) NOT NULL,
  email VARCHAR(255) NOT NULL,
  password VARCHAR(255) NOT NULL, 
  rol ENUM('admin', 'usuario') NOT NULL DEFAULT 'usuario',
  fecha_registro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  UNIQUE INDEX email_UNIQUE (email ASC)
) ENGINE = InnoDB;

-- -----------------------------------------------------
-- 3. Diseños
-- -----------------------------------------------------
CREATE TABLE disenos (
  id INT NOT NULL AUTO_INCREMENT,
  nombre VARCHAR(255) NOT NULL,
  precio DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
  tamano_taza_id INT NOT NULL,
  descripcion TEXT NULL,
  ruta_diseno VARCHAR(512) NULL,
  fecha_creacion DATE NOT NULL,
  publicado BOOLEAN NOT NULL DEFAULT FALSE,
  PRIMARY KEY (id),
  UNIQUE INDEX ruta_diseno_UNIQUE (ruta_diseno ASC),
  CONSTRAINT fk_disenos_tamanos_taza
    FOREIGN KEY (tamano_taza_defecto_id)
    REFERENCES tamanos_taza (id)
    ON DELETE RESTRICT
    ON UPDATE CASCADE
) ENGINE = InnoDB;

-- -----------------------------------------------------
-- 4. Tags y su relación N:M con Diseños
-- -----------------------------------------------------
CREATE TABLE tags (
  id INT NOT NULL AUTO_INCREMENT,
  nombre VARCHAR(100) NOT NULL,
  PRIMARY KEY (id),
  UNIQUE INDEX nombre_UNIQUE (nombre ASC)
) ENGINE = InnoDB;

CREATE TABLE diseno_tags (
  diseno_id INT NOT NULL,
  tag_id INT NOT NULL,
  PRIMARY KEY (diseno_id, tag_id),
  CONSTRAINT fk_diseno_tags_disenos
    FOREIGN KEY (diseno_id)
    REFERENCES disenos (id)
    ON DELETE CASCADE,
  CONSTRAINT fk_diseno_tags_tags
    FOREIGN KEY (tag_id)
    REFERENCES tags (id)
    ON DELETE CASCADE
) ENGINE = InnoDB;

-- -----------------------------------------------------
-- 5. Direcciones de Envío
-- -----------------------------------------------------
CREATE TABLE direcciones (
  id INT NOT NULL AUTO_INCREMENT,
  usuario_id INT NOT NULL,
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
  CONSTRAINT fk_direcciones_usuarios
    FOREIGN KEY (usuario_id)
    REFERENCES usuarios (id)
    ON DELETE CASCADE
) ENGINE = InnoDB;

-- -----------------------------------------------------
-- 6. Carrito de Compras (Temporal)
-- -----------------------------------------------------
CREATE TABLE carrito_items (
  usuario_id INT NOT NULL,
  diseno_id INT NOT NULL,
  tamano_taza_id INT NOT NULL,
  cantidad INT NOT NULL DEFAULT 1,
  fecha_agregado DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (usuario_id, diseno_id, tamano_taza_id),
  CONSTRAINT fk_carrito_usuarios FOREIGN KEY (usuario_id) REFERENCES usuarios (id) ON DELETE CASCADE,
  CONSTRAINT fk_carrito_disenos FOREIGN KEY (diseno_id) REFERENCES disenos (id) ON DELETE CASCADE,
  CONSTRAINT fk_carrito_tamanos FOREIGN KEY (tamano_taza_id) REFERENCES tamanos_taza (id) ON DELETE RESTRICT
) ENGINE = InnoDB;

-- -----------------------------------------------------
-- 7. Pedidos (Histórico de Ventas)
-- -----------------------------------------------------
CREATE TABLE pedidos (
  id INT NOT NULL AUTO_INCREMENT,
  folio_pedido VARCHAR(100) NOT NULL,
  usuario_id INT NOT NULL,
  monto_total DECIMAL(10, 2) NOT NULL,
  estatus ENUM('pendiente', 'pagado', 'enviado', 'entregado', 'cancelado') DEFAULT 'pendiente',
  fecha_pedido DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  UNIQUE INDEX folio_UNIQUE (folio_pedido ASC),
  CONSTRAINT fk_pedidos_usuarios FOREIGN KEY (usuario_id) REFERENCES usuarios (id) ON DELETE RESTRICT
) ENGINE = InnoDB;

-- -----------------------------------------------------
-- 8. Detalle del Pedido
-- -----------------------------------------------------
CREATE TABLE pedido_items (
  id INT NOT NULL AUTO_INCREMENT,
  pedido_id VARCHAR(100) NOT NULL,
  diseno_id INT NOT NULL,
  tamano_taza_id INT NOT NULL,
  precio DECIMAL(10, 2) NOT NULL, -- Precio al que se vendió en ese momento
  cantidad INT NOT NULL DEFAULT 1,
  PRIMARY KEY (id),
  CONSTRAINT fk_items_pedido FOREIGN KEY (pedido_id) REFERENCES pedidos (id) ON DELETE CASCADE,
  CONSTRAINT fk_items_diseno FOREIGN KEY (diseno_id) REFERENCES disenos (id) ON DELETE RESTRICT,
  CONSTRAINT fk_items_tamano FOREIGN KEY (tamano_taza_id) REFERENCES tamanos_taza (id) ON DELETE RESTRICT
) ENGINE = InnoDB;