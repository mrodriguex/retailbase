import * as signalR from "@microsoft/signalr";
import { getToken, getUsername } from "./authService";
import { env } from "../config";

let connection = null;

function buildHubUrl() {
  const hubUrl = new URL(env.signalrHubUrl, window.location.origin);
  const username = getUsername()?.trim();

  if (username) {
    hubUrl.searchParams.set("userName", username);
  }

  return hubUrl.toString();
}

export function getDataHubConnection() {
  if (connection) return connection;

  connection = new signalR.HubConnectionBuilder()
    .withUrl(buildHubUrl(), {
      accessTokenFactory: () => getToken() || "",
      withCredentials: false,
      transport: signalR.HttpTransportType.WebSockets,
      skipNegotiation: env.signalrSkipNegotiation,
    })
    .withAutomaticReconnect([0, 2000, 5000, 10000])
    .build();

  return connection;
}

export async function startDataHub() {
  const conn = getDataHubConnection();
  if (conn.state === signalR.HubConnectionState.Disconnected) {
    await conn.start();
  }
  return conn;
}

export async function stopDataHub() {
  if (!connection) return;

  try {
    if (connection.state !== signalR.HubConnectionState.Disconnected) {
      await connection.stop();
    }
  } finally {
    // Force a fresh connection instance so URL/query params are rebuilt per login.
    connection = null;
  }
}

async function ensureConnected() {
  const conn = getDataHubConnection();
  if (conn.state === signalR.HubConnectionState.Disconnected) {
    await conn.start();
  }
  return conn;
}

export async function sendBroadcastMessage(message) {
  const conn = await ensureConnected();
  await conn.invoke("SendMessage", message);
}

export async function sendMessageToUser(userId, message) {
  const conn = await ensureConnected();
  await conn.invoke("SendMessageToUser", userId, message);
}