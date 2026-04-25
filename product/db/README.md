# Local Infrastructure

## PostgreSQL

Start the database stack:

```bash
docker compose -f infra/docker-compose.yml up -d postgres
```

## LDAP

Start the LDAP test stack:

```bash
docker compose -f infra/docker-compose.yml up -d ldap ldap-seed
```

## Full Stack

Start backend, frontend, PostgreSQL, and LDAP together:

```bash
docker compose -f infra/docker-compose.yml up --build -d
```

Start the optional admin tools as well:

```bash
docker compose -f infra/docker-compose.yml --profile tools up --build -d
```

LDAP connection details for the backend:

- Host: `localhost`
- Port: `389`
- Bind DN: `cn=admin,dc=planetexpress,dc=com`
- Bind password: `GoodNewsEveryone`
- Search base: `ou=people,dc=planetexpress,dc=com`
- User filter: `(uid={0})`

Seeded test users:

- `developer` / `developer`
- `admin` / `admin`

Notes:

- `ldap-seed` is a one-shot job that creates `ou=people` and the test users automatically.
- After a full reset, wait until `ldap` is healthy and `ldap-seed` has exited successfully.

Clean reset:

```bash
docker compose -f infra/docker-compose.yml down
docker compose -f infra/docker-compose.yml up -d ldap ldap-seed
```

Available URLs:

- pgAdmin: `http://localhost:8080`
- phpLDAPadmin: `http://localhost:8081`
- Frontend: `http://localhost:4200`
- Backend API: `http://localhost:5001`
