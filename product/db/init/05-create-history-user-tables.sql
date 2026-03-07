BEGIN;

CREATE TABLE users
(
    id           SERIAL PRIMARY KEY,
    external_id  TEXT NOT NULL,
    username     TEXT NOT NULL,

    CONSTRAINT uq_users_external_id UNIQUE (external_id)
);

CREATE TABLE event_history
(
    id          SERIAL PRIMARY KEY,
    event_id    INT NOT NULL,
    user_id     INT NOT NULL,
    action      TEXT NOT NULL,
    created_at  TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_event_history_event
        FOREIGN KEY (event_id)
        REFERENCES event(id)
        ON DELETE CASCADE,

    CONSTRAINT fk_event_history_user
        FOREIGN KEY (user_id)
        REFERENCES users(id)
        ON DELETE RESTRICT
);

CREATE INDEX ix_event_history_event_id
    ON event_history(event_id);

CREATE INDEX ix_event_history_user_id
    ON event_history(user_id);

CREATE INDEX ix_event_history_created_at
    ON event_history(created_at);

CREATE INDEX ix_event_history_event_id_created_at
    ON event_history(event_id, created_at DESC);

COMMIT;