#!/usr/bin/env bash

if [[ $(git diff --stat) != '' ]]; then
  echo 'Working tree contains changes, commit first please'
  exit
fi

current=$(dotnet gitversion /showvariable MajorMinorPatch)
major=$(dotnet gitversion /showvariable Major)
minor=$(dotnet gitversion /showvariable Minor)
patch=$(dotnet gitversion /showvariable Patch)

echo $patch
nextv=$((patch + 1))

#release="${major}.${minor}.${nextv}"
echo Creating release $release
release="${current}"

patchlevel=patch
if [ "$1" != "" ]; then
  patchlevel=$1
fi

echo Current version is $current
echo New release is $release

git flow release start $release
git push origin release/$release

export GIT_MERGE_AUTOEDIT=no
gitversion /updateassemblyinfo

git commit -am "Updating assembly for ${release}"
git flow release finish $release -m "Release: ${release}"
unset GIT_MERGE_AUTOEDIT

git push origin trunk develop
