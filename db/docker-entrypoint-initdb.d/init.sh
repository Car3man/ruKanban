#!/bin/bash
set -e

PGUSERNAME="postgres"
PGPASSWORD="postgres"
PGDATABASE="postgres"

psql -v ON_ERROR_STOP=1 -d $PGDATABASE -U $PGUSERNAME <<-EOSQL

-- Creating user

CREATE USER rukanban_admin WITH PASSWORD '123456' CREATEDB;

-- Creating db

CREATE DATABASE rukanban OWNER rukanban_admin;

EOSQL

PGUSERNAME="rukanban_admin"
PGPASSWORD="123456"
PGDATABASE="rukanban"

psql -v ON_ERROR_STOP=1 -d $PGDATABASE -U $PGUSERNAME <<-EOSQL

-- Not affect in docker

DROP TABLE IF EXISTS public.users;
DROP TABLE IF EXISTS public.refresh_tokens;
DROP TABLE IF EXISTS public.roles;

-- Creating tables

CREATE TABLE public.users
(
    id BIGSERIAL NOT NULL,
    login CHARACTER VARYING(36) NOT NULL,
    password_hash CHARACTER VARYING(32) NOT NULL,
    first_name CHARACTER VARYING(36) NOT NULL,
    sur_name CHARACTER VARYING(36) NOT NULL,
    patronymic CHARACTER VARYING(36) NOT NULL,
	role_id BIGINT NOT NULL,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    PRIMARY KEY (id),
    UNIQUE (login)
);
ALTER TABLE IF EXISTS public.users
    OWNER to rukanban_admin;

CREATE TABLE public.refresh_tokens
(
    id BIGSERIAL NOT NULL,
    token TEXT NOT NULL,
    user_id BIGINT NOT NULL,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    expires_at TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    PRIMARY KEY (id),
    UNIQUE (token)
);
ALTER TABLE IF EXISTS public.refresh_tokens
    OWNER to rukanban_admin;

CREATE TABLE public.revoked_tokens
(
    id BIGSERIAL NOT NULL,
    token TEXT NOT NULL,
    revoked_at TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    PRIMARY KEY (id),
    UNIQUE (token)
);
ALTER TABLE IF EXISTS public.revoked_tokens
    OWNER to rukanban_admin;

CREATE TABLE public.roles
(
    id BIGSERIAL NOT NULL,
    name CHARACTER VARYING(36) NOT NULL,
    PRIMARY KEY (id),
    UNIQUE (name)
);
ALTER TABLE IF EXISTS public.roles
    OWNER to rukanban_admin;

CREATE TABLE public.workspaces
(
    id BIGSERIAL NOT NULL,
    name CHARACTER VARYING(36) NOT NULL,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE public.boards
(
    id BIGSERIAL NOT NULL,
    workspace_id BIGINT NOT NULL,
    name CHARACTER VARYING(36) NOT NULL,
    description CHARACTER VARYING(36) NOT NULL,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE public.columns
(
    id BIGSERIAL NOT NULL,
    board_id BIGINT NOT NULL,
    index int NOT NULL,
    name CHARACTER VARYING(36) NOT NULL,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    PRIMARY KEY (id),
    UNIQUE (index)
);

CREATE TABLE public.tickets
(
    id BIGSERIAL NOT NULL,
    columnd_id BIGINT NOT NULL,
    index int NOT NULL,
    title CHARACTER VARYING(128) NOT NULL,
    description CHARACTER VARYING(2048) NOT NULL,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    PRIMARY KEY (id),
    UNIQUE (index)
);

CREATE TABLE public.workspace_roles
(
    id BIGSERIAL NOT NULL,
    name CHARACTER VARYING(36) NOT NULL,
    PRIMARY KEY (id),
    UNIQUE (name)
);

CREATE TABLE public.user_workspace
(
    id BIGSERIAL NOT NULL,
    user_id BIGINT NOT NULL,
    workspace_id BIGINT NOT NULL,
    workspace_role_id BIGINT NOT NULL,
    PRIMARY KEY (id),
    UNIQUE (user_id, workspace_id)
);

CREATE TABLE public.user_board
(
    id BIGSERIAL NOT NULL,
    user_id BIGINT NOT NULL,
    board_id BIGINT NOT NULL,
    PRIMARY KEY (id),
    UNIQUE (user_id, board_id)
);

CREATE TABLE public.user_ticket
(
    id BIGSERIAL NOT NULL,
    user_id BIGINT NOT NULL,
    ticket_id BIGINT NOT NULL,
    PRIMARY KEY (id),
    UNIQUE (user_id, ticket_id)
);

-- Relationships
	
ALTER TABLE public.refresh_tokens 
    ADD CONSTRAINT fk_refresh_tokens_users
    FOREIGN KEY (user_id) 
    REFERENCES public.users (id);

ALTER TABLE public.users 
    ADD CONSTRAINT fk_users_roles
    FOREIGN KEY (role_id) 
    REFERENCES public.roles (id);

ALTER TABLE public.tickets
    ADD CONSTRAINT fk_tickets_columns
    FOREIGN KEY (columnd_id)
    REFERENCES public.columns (id);

ALTER TABLE public.columns
    ADD CONSTRAINT fk_columns_boards
    FOREIGN KEY (board_id)
    REFERENCES public.boards (id);

ALTER TABLE public.boards
    ADD CONSTRAINT fk_boards_workspaces
    FOREIGN KEY (workspace_id)
    REFERENCES public.workspaces (id);

ALTER TABLE public.user_workspace
    ADD CONSTRAINT fk_user_workspace_users
    FOREIGN KEY (user_id)
    REFERENCES public.users (id);

ALTER TABLE public.user_workspace
    ADD CONSTRAINT fk_user_workspace_workspaces
    FOREIGN KEY (workspace_id)
    REFERENCES public.workspaces (id);

ALTER TABLE public.user_workspace
    ADD CONSTRAINT fk_user_workspaces_workspace_roles
    FOREIGN KEY (workspace_role_id)
    REFERENCES public.workspace_roles (id);

ALTER TABLE public.user_board
    ADD CONSTRAINT fk_user_board_users
    FOREIGN KEY (user_id)
    REFERENCES public.users (id);

ALTER TABLE public.user_board
    ADD CONSTRAINT fk_user_board_boards
    FOREIGN KEY (board_id)
    REFERENCES public.boards (id);

ALTER TABLE public.user_ticket
    ADD CONSTRAINT fk_user_ticket_users
    FOREIGN KEY (user_id)
    REFERENCES public.users (id);

ALTER TABLE public.user_ticket
    ADD CONSTRAINT fk_user_ticket_tickets
    FOREIGN KEY (ticket_id)
    REFERENCES public.tickets (id);

-- Init data

INSERT INTO public.roles (name) VALUES ('root');
INSERT INTO public.roles (name) VALUES ('user');

INSERT INTO public.workspace_roles (name) VALUES ('owner');
INSERT INTO public.workspace_roles (name) VALUES ('user');

EOSQL