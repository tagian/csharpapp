#!/usr/bin/env bash

set -e

echo "The following directories will be deleted:"

find . \
  -type d \
  \( -name bin -o -name obj \) \
  -prune \
  -print

read -rp "Continue? [y/N] " answer

if [[ "$answer" != "y" && "$answer" != "Y" ]]; then
  echo "Cancelled."
  exit 0
fi

find . \
  -type d \
  \( -name bin -o -name obj \) \
  -prune \
  -exec rm -rf {} +

echo "Clean complete."