#!/usr/bin/env bash

docker --context noodles compose pull && \
    docker --context noodles compose down && \
    docker --context noodles compose up -d && \
    docker --context noodles compose logs -f
