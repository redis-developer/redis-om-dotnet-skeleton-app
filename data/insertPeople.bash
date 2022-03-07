#!/bin/bash
curl -d @HarryPotter.json -X POST http://localhost:5000/people -H 'Content-Type: application/json'; echo
curl -d @Holmes.json -X POST http://localhost:5000/people -H 'Content-Type: application/json'; echo
curl -d @KateBishop.json -X POST http://localhost:5000/people -H 'Content-Type: application/json'; echo
curl -d @Monica.json -X POST http://localhost:5000/people -H 'Content-Type: application/json'; echo
curl -d @TonyStark.json -X POST http://localhost:5000/people -H 'Content-Type: application/json'; echo