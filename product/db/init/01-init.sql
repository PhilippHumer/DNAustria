-- =========================================
-- Tables
-- =========================================
CREATE EXTENSION IF NOT EXISTS citext;

CREATE TABLE IF NOT EXISTS address (
    id              SERIAL PRIMARY KEY,
    street          VARCHAR(50) NOT NULL,
    city            VARCHAR(50) NOT NULL,
    zip             VARCHAR(10) NOT NULL,
    state           VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS organization (
    id              SERIAL PRIMARY KEY,
    name            VARCHAR(50) NOT NULL,
    adress          INT REFERENCES address(id)
);

CREATE DOMAIN EMAIL AS CITEXT
CHECK (
  VALUE ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$'
);

CREATE TABLE IF NOT EXISTS contact (
    id              SERIAL PRIMARY KEY,
    name            VARCHAR(50) NOT NULL,
    email           EMAIL NOT NULL UNIQUE,
    phone           TEXT NOT NULL UNIQUE,
    organization    VARCHAR(50) NULL
);

CREATE TABLE IF NOT EXISTS location (
    id              SERIAL PRIMARY KEY,
    name            VARCHAR(50) NOT NULL,
    address         INT REFERENCES address(id),
    latitude        FLOAT NOT NULL,
    longitude       FLOAT NOT NULL
);

CREATE DOMAIN AGE AS INT
CHECK (
    VALUE >= 0 AND VALUE <= 999
);

CREATE TABLE IF NOT EXISTS event (
    id              SERIAL PRIMARY KEY,
    name            VARCHAR(50) NOT NULL,
    description     TEXT NOT NULL,
    link            VARCHAR(200) NOT NULL,
    start_date      TIMESTAMP NOT NULL,
    end_date        TIMESTAMP NOT NULL,
    classification  INT NOT NULL, -- scheduled/on-demand
    status          INT NOT NULL, -- internal status
    has_fees        BOOLEAN NOT NULL,
    is_online       BOOLEAN NOT NULL,
    organization    INT REFERENCES organization(id),
    program_name    VARCHAR(50) NOT NULL,
    format          VARCHAR(100) NOT NULL,
    school_bookable BOOLEAN NOT NULL,
    age_minimum     AGE NOT NULL,
    age_maximum     AGE NOT NULL,
    location        INT REFERENCES location(id),
    contact         INT REFERENCES contact(id)
);

CREATE TABLE IF NOT EXISTS event_target_audience (
    event           INT REFERENCES event(id),
    target_audience INT NOT NULL,
    PRIMARY KEY     (event, target_audience)
);

CREATE TABLE IF NOT EXISTS event_topic (
    event           INT REFERENCES event(id),
    topic           INT NOT NULL,
    PRIMARY KEY     (event, topic)
);

