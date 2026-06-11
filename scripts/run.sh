#!/usr/bin/env bash
set -euo pipefail

ORBI_PROJECT="src/Orbi.Web/Orbi.Web.csproj"
POSTGRES_PORT=5432

# ─── Utility ──────────────────────────────────────────────────────────────────

ensure_docker_daemon() {
    if ! systemctl is-active --quiet docker 2>/dev/null; then
        echo "[*] Docker daemon is not running. Starting and enabling it..."
        sudo systemctl enable --now docker
    fi
}

stop_docker_daemon() {
    if systemctl is-active --quiet docker 2>/dev/null; then
        echo "[*] Stopping and disabling Docker daemon..."
        sudo systemctl disable --now docker
    fi
}

kill_process_on_port() {
    local port=$1
    local pids
    pids=$(lsof -ti :"$port" 2>/dev/null || true)
    if [[ -n "$pids" ]]; then
        echo "[*] Killing process(es) on port $port (PID: $pids)"
        kill -9 "$pids" 2>/dev/null || true
        sleep 1
    fi
}

kill_dotnet_watch() {
    local pids
    pids=$(pgrep -f "dotnet.*watch" 2>/dev/null || true)
    if [[ -n "$pids" ]]; then
        echo "[*] Killing dotnet watch processes (PID: $pids)"
        kill -9 "$pids" 2>/dev/null || true
        sleep 1
    fi
}

# ─── Database only ────────────────────────────────────────────────────────────

db_up() {
    ensure_docker_daemon
    kill_process_on_port "$POSTGRES_PORT"
    echo "[+] Starting PostgreSQL container..."
    docker compose up -d --wait
    echo "[+] PostgreSQL is ready."
}

db_down() {
    echo "[+] Stopping PostgreSQL container..."
    docker compose down 2>/dev/null || true
    stop_docker_daemon
    echo "[+] PostgreSQL stopped and Docker disabled."
}

# ─── Full app ─────────────────────────────────────────────────────────────────

up() {
    db_up
    echo "[+] Starting Orbi in watch mode..."
    dotnet watch run --project "$ORBI_PROJECT"
}

down() {
    echo "[+] Stopping all containers..."
    docker compose down 2>/dev/null || true
    kill_dotnet_watch
    kill_process_on_port "$POSTGRES_PORT"
    stop_docker_daemon
    echo "[+] All processes stopped and Docker disabled."
}

clean() {
    echo "[!] This will destroy all persisted data (volumes)."
    read -rp "  Are you sure? (yes/NO): " confirm
    if [[ "$confirm" != "yes" ]]; then
        echo "[-] Aborted."
        exit 0
    fi

    ensure_docker_daemon
    down
    echo "[+] Removing volumes and orphans..."
    docker compose down -v --remove-orphans 2>/dev/null || true
    kill_process_on_port "$POSTGRES_PORT"
    kill_dotnet_watch

    echo "[+] Clean slate ready. Starting fresh..."
    up
}

# ─── Commit ───────────────────────────────────────────────────────────────────

commit() {
    clear
    echo ""
    echo "╔══════════════════════════════════════╗"
    echo "║       Semantic Commit                ║"
    echo "╠══════════════════════════════════════╣"
    echo "║  1  feat                             ║"
    echo "║  2  fix                              ║"
    echo "║  3  docs                             ║"
    echo "║  b  Back to main menu                ║"
    echo "╚══════════════════════════════════════╝"
    echo ""
    read -rp "Choose commit type: " type_choice
    echo ""

    case "$type_choice" in
        1) type="feat" ;;
        2) type="fix" ;;
        3) type="docs" ;;
        b|B) menu; return ;;
        *) echo "[-] Invalid option."; commit; return ;;
    esac

    read -rp "Commit message: " message
    if [[ -z "$message" ]]; then
        echo "[-] Message cannot be empty."
        commit
        return
    fi

    git add .
    git commit -m "$type: $message"
    echo "[+] Pushing to remote..."
    git pull origin main && git push origin main
    echo ""
    read -rp "Press Enter to continue..." _
    menu
}

# ─── Menu ─────────────────────────────────────────────────────────────────────

menu() {
    clear
    echo ""
    echo "╔══════════════════════════════════════╗"
    echo "║        Orbi Control Panel            ║"
    echo "╠══════════════════════════════════════╣"
    echo "║  1  Start full app                   ║"
    echo "║  2  Stop full app                    ║"
    echo "║  3  Database up                      ║"
    echo "║  4  Database down                    ║"
    echo "║  5  Clean and run fresh              ║"
    echo "║  6  Commit                           ║"
    echo "║  q  Quit                             ║"
    echo "╚══════════════════════════════════════╝"
    echo ""
    read -rp "Choose an option: " choice
    echo ""

    case "$choice" in
        1) up ;;
        2) down ;;
        3) db_up ;;
        4) db_down ;;
        5) clean ;;
        6) commit ;;
        q|Q) echo "Bye."; exit 0 ;;
        *) echo "[-] Invalid option."; menu ;;
    esac
}

# ─── Main ─────────────────────────────────────────────────────────────────────

main() {
    cd "$(dirname "$0")/.."
    echo "[i] Working directory: $(pwd)"

    if ! command -v docker &>/dev/null; then
        echo "[-] Docker is not installed."
        exit 1
    fi

    if ! command -v dotnet &>/dev/null; then
        echo "[-] .NET SDK is not installed."
        exit 1
    fi

    if [[ $EUID -eq 0 ]]; then
        echo "[-] Do not run as root."
        exit 1
    fi

    menu
}

main
