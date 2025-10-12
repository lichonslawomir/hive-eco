#!/bin/sh

# Variables -- change these!
APP_NAME="Hive.Gateway.Service"
DOTNET_PROJECT_PATH="./HiveAssistant/web/Hive.Gateway.Service/Hive.Gateway.Service.csproj"  # Path to your .csproj
DEPLOY_PATH="/etc/hive/$APP_NAME"
SERVICE_USER="root"  # User to run the service as

# Publish the app
dotnet publish "$DOTNET_PROJECT_PATH" -c Release -r linux-musl-x64 --self-contained true -o "$DEPLOY_PATH"

# Create a user if needed
if ! id "$SERVICE_USER" >/dev/null 2>&1; then
    adduser -D $SERVICE_USER
fi

# Set permissions
chown -R $SERVICE_USER:$SERVICE_USER "$DEPLOY_PATH"

# Create systemd service file (if systemd is available)
if command -v systemctl >/dev/null 2>&1; then
    SERVICE_FILE="/etc/systemd/system/$APP_NAME.service"
    cat > "$SERVICE_FILE" <<EOF
[Unit]
Description=$APP_NAME service
After=network.target

[Service]
Type=simple
WorkingDirectory=$DEPLOY_PATH
Environment="ASPNETCORE_ENVIRONMENT=Production"
Environment="ASPNETCORE_URLS=http://*:80"
ExecStart=/home/share/dotnet/dotnet $DEPLOY_PATH/$APP_NAME.dll
User=$SERVICE_USER
Restart=always
RestartSec=10

[Install]
WantedBy=multi-user.target
EOF

    # Enable & start service
    systemctl daemon-reload
    systemctl enable $APP_NAME
    cp ../appsettings.Production.json /etc/hive/Hive.Gateway.Service/
    systemctl restart $APP_NAME
    echo "Service $APP_NAME started using systemd."

else
    # OpenRC fallback (Alpine default)
    SERVICE_FILE="/etc/init.d/$APP_NAME"
    cat > "$SERVICE_FILE" <<EOF
#!/sbin/openrc-run

export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://*:80"

command="/home/share/dotnet/dotnet"
command_args="$DEPLOY_PATH/$APP_NAME.dll"
directory="$DEPLOY_PATH"
user="$SERVICE_USER"
pidfile="/var/run/$APP_NAME.pid"
name="$APP_NAME"

depend() {
    need net
}
EOF

    chmod +x "$SERVICE_FILE"
    rc-update add $APP_NAME default
    cp ../appsettings.Production.json /etc/hive/Hive.Gateway.Service/
    rc-service $APP_NAME restart
    echo "Service $APP_NAME started using OpenRC."
fi