# Local Infrastructure

## PostgreSQL

Start the database stack:

```bash
docker compose -f db/docker-compose.yml up -d postgres pgadmin
```

## LDAP

Start the LDAP test stack:

```bash
docker compose -f db/docker-compose.yml up -d ldap phpldapadmin
```

LDAP connection details for the backend:

- Host: `localhost`
- Port: `389`
- Bind DN: `cn=admin,dc=example,dc=org`
- Bind password: `admin`
- Search base: `ou=people,dc=example,dc=org`
- User filter: `(uid={0})`

Seeded test users:

- `developer` / `developer`
- `admin` / `admin`

Optional web UI:

- phpLDAPadmin: `http://localhost:8081`

To test the real LDAP path, switch `backend/DNAustria/DNAustria.Api/appsettings.Development.json`:

- `Authentication:Mode` from `Mock` to `Ldap`
