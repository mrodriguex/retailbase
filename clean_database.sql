-- PostgreSQL script to clean all database tables
-- This script truncates all tables in the correct order to handle foreign key constraints
-- Using CASCADE to automatically truncate dependent tables

TRUNCATE "CompanyUser", "MenuItemRole", "RoleUser", "ProductPresentations", "Products", "Brands", "Categories", "Companys", "Customers", "MenuItems", "Roles", "Users" CASCADE;

-- Note: TRUNCATE with CASCADE will reset any associated sequences (auto-increment columns) to their starting values