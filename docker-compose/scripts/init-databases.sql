-- Multi-Site Database Initialization Script
-- This script creates separate databases for each site.
-- It is mounted to /docker-entrypoint-initdb.d/ in the PostgreSQL container
-- and will only run on first startup when the data directory is empty.
--
-- On existing deployments, this script will NOT modify existing databases.
-- To add a new site, create a new script file (e.g., init-databases-site3.sql)
-- and temporarily remove the postgres_data volume to reinitialize.

-- Create database for gg site (ggpaintings.com)
SELECT 'CREATE DATABASE artgallery_gg'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'artgallery_gg')\gexec

-- Create database for flynn site (flynnart.com)
SELECT 'CREATE DATABASE artgallery_flynn'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'artgallery_flynn')\gexec
