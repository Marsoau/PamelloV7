set -euo pipefail

REPO="Marsoau/PamelloV7"
RID="linux-x64"
PRERELEASES=true


DIR="$(cd "$(dirname "$(readlink -f "$0")")" && pwd)"

for c in curl unzip; do
    command -v "$c" >/dev/null || { echo "Missing required tool: $c" >&2; exit 1; }
done

if [[ "$PRERELEASES" == true ]]; then
    API="https://api.github.com/repos/$REPO/releases?per_page=1"
else
    API="https://api.github.com/repos/$REPO/releases/latest"
fi
JSON="$(curl -fsSL "$API")"

TAG="$(printf '%s' "$JSON" | grep -m1 '"tag_name"'         | sed -E 's/.*"([^"]+)".*/\1/' || true)"
URL="$(printf '%s' "$JSON" | grep '"browser_download_url"' | sed -E 's/.*"(https[^"]+)".*/\1/' | grep -m1 "$RID\.zip$" || true)"
[[ -n "$TAG" && -n "$URL" ]] || { echo "Could not find a $RID release for $REPO." >&2; exit 1; }

CURRENT=""; [[ -x "$DIR/PamelloV7.Server" ]] && CURRENT="$("$DIR/PamelloV7.Server" --version 2>/dev/null || true)"

[[ "$CURRENT" == "$TAG" ]] && { echo "Already up to date ($TAG)."; exit 0; }

echo "New version: ${CURRENT:-none} -> $TAG"
echo "This will DELETE everything in: $DIR"
read -rp "Continue? [y/N] " ans
[[ "$ans" == [yY] ]] || { echo "Aborted."; exit 0; }

TMP="$(mktemp -d)"; trap 'rm -rf "$TMP"' EXIT
echo "Downloading $TAG ..."
curl -fL --progress-bar "$URL" -o "$TMP/release.zip"
unzip -q "$TMP/release.zip" -d "$TMP/out"

echo "Installing ..."
find "$DIR" -mindepth 1 -delete

mv "$TMP"/out/*/* "$DIR/"
chmod +x "$DIR/PamelloV7.Server" "$DIR"/*.sh 2>/dev/null || true

echo "Done. Now on $TAG"
