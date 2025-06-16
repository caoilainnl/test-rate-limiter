# Test ts
tsx test-rate-limiter.ts
npm install

# Test python
python3 -m venv .env
source .env/bin/activate
pip install -r requirements.txt
python test-rate-limiter.py

# Test PHP
composer install
php test-rate-limiter.php

# Test CSharp
cd TestThrottler
dotnet run

# Test GO
go mod tidy 
go run test-rate-limiter.go