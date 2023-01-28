require('dotenv').config();

const express = require('express');

const authRoutes = require('./auth_rotes/auth');
const workspaceRoutes = require('./workspace_routes/workspace');
const boardRoutes = require('./board_routes/board');
const columnRoutes = require('./column_routes/column');
const ticketRoutes = require('./ticket_routes/ticket');

const { responseHelper } = require('./common/helpers');

const app = express();

app.use(express.urlencoded({ extended: false }));
app.use(express.json());

app.use(authRoutes);
app.use(workspaceRoutes);
app.use(boardRoutes);
app.use(columnRoutes);
app.use(ticketRoutes);
app.all('*', (req, res) => responseHelper.sendNotFound(req, res));

module.exports = app;
