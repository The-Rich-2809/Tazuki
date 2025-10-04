CREATE DATABASE IF NOT EXISTS disenos_db DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;

  USE disenos_db;

  DROP TABLE IF EXISTS diseno_tags;
  DROP TABLE IF EXISTS disenos;
  DROP TABLE IF EXISTS tags;
  DROP TABLE IF EXISTS tamanos_taza;

  CREATE TABLE tamanos_taza (
    id INT NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(255) NOT NULL,
    PRIMARY KEY (id),
    UNIQUE INDEX nombre_UNIQUE (nombre ASC)
  ) ENGINE = InnoDB;

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