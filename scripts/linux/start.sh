cd "$(dirname "$0")"

chmod +x ./PamelloV7.Server 2>/dev/null || true
./PamelloV7.Server "$@"
code=$?

echo ""
if [ "$code" -eq 0 ]; then
  echo "PamelloV7.Server exited normally (code 0)."
else
  echo "PamelloV7.Server exited with code $code."
  printf "Press Enter to close... "
  read _
fi
