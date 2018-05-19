## How to use this sample

This sample demonstrates how to use the Vault configuration provider. However, you wil also need to configure vault. 

The following guide describes how to run Vault in dev mode, create the test secret, add an AppRole and configure the required secrets:

1. Start the server in dev mode: 

```bash
vault server -dev -dev-root-token-id="root"
```

Verify the status

```bash
set VAULT_ADDR=http://127.0.0.1:8200
vault status
```

2. Create a secret that we'll retrieve in the sample
 
```bash
vault kv put secret/sampleapp/thesecret foo=world excited=yes testlocation=thevalue
```

3. Enable approle auth method 

```bash
vault auth enable approle
```

4. Create a policy giving access to the secrets required by the sample app

Either loading from an .hcl file (in the project root):

```bash
vault policy write sampleapp sampleapp-pol.hcl
```
or
```bash
cat sampleapp-pol.hcl | vault policy write sampleapp -
```

or using HEREDOC in bash:
```bash
vault policy write sampleapp -<<'EOF'
# Login with AppRole
path "auth/approle/login" {
  capabilities = [ "create", "read" ]
}

# Read test data
path "secret/data/sampleapp/*" {
  capabilities = [ "read" ]
}
EOF
```

> Note, if you are using version 2 of the kv secrets again (the default in Vault 0.10.0+), you need to provide permisions to the `secret/data/*` path, rather than just `secret/data`. You will also need to pass this location in the `UseVault` extension method.

5. Create a new role, **sampleapp-role** and attach the policy 

```bash
vault write auth/approle/role/sampleapp-role policies="sampleapp"
```

6. Get the RoleId for the role

```bash
vault read auth/approle/role/sampleapp-role/role-id
```
This will generate a UUID:

```bash
Key        Value
---        -----
role_id    5b391a7a-8a09-5dd6-f76c-a34d65736f1f
```

7. Generate a new Secret ID for the role

```bash
vault write -f auth/approle/role/sampleapp-role/secret-id
```

This will generate a SecretId and a SecretID Accessor:

```bash
Key                   Value
---                   -----
secret_id             3a80eab2-ae81-40e9-072d-853a5af6b2b2
secret_id_accessor    4d0fe8b2-b1ab-6464-0026-d8700efd085b
```

8. Make the RoleId and SecretId available to the app as configuration values. You should inject these into your app in a secure way, for example using environment variables