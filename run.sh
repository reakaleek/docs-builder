#!/bin/sh

set -euo pipefail

# guess OS_TYPE if not provided
OS_TYPE=${OS_TYPE:-}
if [ -z "$OS_TYPE" ]; then
  case "$(uname -s | tr '[:upper:]' '[:lower:]')" in
    cygwin_nt*|mingw*|msys_nt*)
      OS_TYPE="windows"
      ;;
    linux*)
      OS_TYPE="linux"
      ;;
    darwin*)
      OS_TYPE="linux"
      ;;
  esac
fi

ARCHITECTURE=${ARCHITECTURE:-}
# guess OS architecture if not provided
if [ -z "$ARCHITECTURE" ]; then
  case $(uname -m) in
    x86_64)  ARCHITECTURE="x64" ;;
    aarch64) ARCHITECTURE="arm64" ;;
    arm64) ARCHITECTURE="arm64" ;;
  esac
fi

docker build --platform="${OS_TYPE}/${ARCHITECTURE}" -f Dockerfile -t docs-builder-${ARCHITECTURE}:latest .
echo "Dockerfile build"

if test -d "../markitpy-poc/docs/source/"; then
  echo "Auto mounting markitpy-poc docs"
  docker run -v "${PWD}/../markitpy-poc/docs/source/:/app/markitpy-poc/docs/source/" -v "${PWD}/docs/:/app/docs/" -v "${PWD}/.artifacts/:/app/.artifacts/" \
    -p 8080:8080 \
    --entrypoint "./docs-builder" "docs-builder-${ARCHITECTURE}:latest" "$@"
else
  docker run -v "${PWD}/docs/:/app/docs/" -v "${PWD}/.artifacts/:/app/.artifacts/" \
    -p 8080:8080 \
    --entrypoint "./docs-builder" "docs-builder-${ARCHITECTURE}:latest" "$@"
fi

