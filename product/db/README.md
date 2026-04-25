# Local Infrastructure

## PostgreSQL

Start the database stack:

```bash
docker compose -f db/docker-compose.yml up -d postgres pgadmin
```

## LDAP

Start the LDAP test stack:

```bash
docker compose -f db/docker-compose.yml up -d ldap ldap-seed phpldapadmin
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
docker compose -f db/docker-compose.yml down
docker compose -f db/docker-compose.yml up -d ldap ldap-seed phpldapadmin
```

Optional web UI:

- phpLDAPadmin: `http://localhost:8081`

To test the real LDAP path, switch `backend/DNAustria/DNAustria.Api/appsettings.Development.json`:

- `Authentication:Mode` from `Mock` to `Ldap`
