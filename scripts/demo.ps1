#!/usr/bin/env pwsh
# Demo script: obtain JWT and call protected POST /api/customers
try {
    $r = Invoke-RestMethod -Method Post -Uri 'http://localhost:5230/api/auth/token' -ContentType 'application/json' -Body (@{username='admin'; password='Password123!'} | ConvertTo-Json)
    if (-not $r -or -not $r.token) {
        Write-Error "Failed to obtain token. Response: $r"
        exit 1
    }
    Write-Output "---TOKEN (truncated)---"
    Write-Output ($r.token.Substring(0,24) + '...')

    $hdr = @{ Authorization = 'Bearer ' + $r.token }
    $body = @{ firstName = 'CLI'; lastName = 'Demo'; email = 'cli.demo@example.com'; phone = '+1-800-0000'; dateOfBirth = '1990-01-01' }

    $created = Invoke-RestMethod -Method Post -Uri 'http://localhost:5230/api/customers' -Headers $hdr -ContentType 'application/json' -Body ($body | ConvertTo-Json)
    Write-Output '---CREATED CUSTOMER---'
    $created | ConvertTo-Json -Depth 5
}
catch {
    Write-Error "Error during demo: $_"
    exit 2
}
