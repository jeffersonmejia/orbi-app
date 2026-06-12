#!/bin/bash
set -euo pipefail

SOLUTION_NAME="Orbi"
PROJECT_NAME="Orbi.Web"
ROOT_DIR="$(dirname "$0")"
PROJECT_DIR="$ROOT_DIR/src/$PROJECT_NAME"

create_solution_and_project() {
    echo "Creating solution and MVC project..."
    dotnet new slnx --name "$SOLUTION_NAME" --output "$ROOT_DIR"
    dotnet new mvc --name "$PROJECT_NAME" --output "$PROJECT_DIR"
    dotnet sln "$ROOT_DIR/$SOLUTION_NAME.slnx" add "$PROJECT_DIR/$PROJECT_NAME.csproj"
}

install_packages() {
    echo "Installing NuGet packages..."
    dotnet add "$PROJECT_DIR/$PROJECT_NAME.csproj" package Npgsql.EntityFrameworkCore.PostgreSQL
    dotnet add "$PROJECT_DIR/$PROJECT_NAME.csproj" package Microsoft.EntityFrameworkCore.Design
    dotnet add "$PROJECT_DIR/$PROJECT_NAME.csproj" package Microsoft.EntityFrameworkCore.Tools
    dotnet add "$PROJECT_DIR/$PROJECT_NAME.csproj" package Microsoft.VisualStudio.Web.CodeGeneration.Design
}

main() {
    echo "=== Orbi - Setup ==="
    create_solution_and_project
    install_packages
    echo "=== Setup complete ==="
}

main
