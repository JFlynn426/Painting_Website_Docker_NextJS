# Complete Guide: Setting Up Yahoo OAuth for Admin Login

This guide walks you through everything needed to get Yahoo OAuth login working, from creating a Yahoo account to configuring the application.

---

## Part 1: Create a Yahoo Account

Since you don't have a Yahoo account, you need to create one first.

### Step 1.1: Navigate to Yahoo Sign-Up
1. Open your web browser
2. Go to: **https://login.yahoo.com/account/create**
3. Fill in the sign-up form:
   - First name
   - Last name
   - Choose your Yahoo email address (e.g., `yourname@yahoo.com`)
   - Create a password
   - Date of birth
   - Email address (for recovery)
4. Click **Agree & Continue**
5. Verify your account through the confirmation email or phone number

### Step 1.2: Sign In
1. Go to **https://www.yahoo.com**
2. Click **Sign In**
3. Enter your new Yahoo email and password
4. You're now logged into Yahoo

---

## Part 2: Register a Yahoo OAuth App

Yahoo uses the **Yahoo Developer Network** to manage OAuth applications.

### Step 2.1: Go to Yahoo Developer Network
1. Open your browser
2. Navigate to: **https://developer.yahoo.com/apps/create/**
3. Sign in with your Yahoo account if prompted

### Step 2.2: Create a New App
1. Click the **Create App** button (or similar)
2. Fill in the app details:
   - **App Name**: Enter a name like `Painting Gallery Admin` (this is just a label for you)
   - **App Type**: Select **Web** or **Server** (not Native)
3. Click **Create App** or **Save**

### Step 2.3: Configure Redirect URI (Callback URL)
This is the **most important step**. The redirect URI tells Yahoo where to send the user after they authorize your app.

1. In your app settings, find the **Redirect URI** or **Callback URL** field
2. Add the following redirect URI based on which site you're configuring:

   **For the `gg` site (port 8181):**
   ```
   https://localhost:8181/admin/login
   ```

   **For the `flynn` site (port 8182):**
   ```
   https://localhost:8182/admin/login
   ```

3. Save the changes

### Step 2.4: Note Your Credentials
After creating the app, Yahoo will show you:
- **Client ID** - A string of characters (e.g., `dj0yJmk9...`)
- **Client Secret** - A longer string (e.g., `xxxx`)

**Copy both values and save them somewhere safe** (like a text file). You'll need them in the next step.

> **Note:** The Client Secret may only be shown once. Make sure to copy it immediately.

---

## Part 3: Configure the Application

Now you need to update the environment configuration with your Yahoo OAuth credentials.

### Step 3.1: Open the .env.multi File
1. Open your code editor (VS Code)
2. Navigate to the file: `docker-compose/.env.multi`
3. Find the **Yahoo OAuth** section (around line 59-62)

### Step 3.2: Update the GG Site Yahoo OAuth Settings
Find these lines:
```
# --- Yahoo OAuth ---
GG_YAHOO_AUTH_CLIENT_ID=dummy_yahoo_client_id_gg
GG_YAHOO_AUTH_CLIENT_SECRET=dummy_yahoo_client_secret_gg
GG_YAHOO_AUTH_REDIRECT_URI=https://localhost:8181/admin/login
```

Replace the dummy values with your actual credentials:
```
# --- Yahoo OAuth ---
GG_YAHOO_AUTH_CLIENT_ID=your_actual_client_id_here
GG_YAHOO_AUTH_CLIENT_SECRET=your_actual_client_secret_here
GG_YAHOO_AUTH_REDIRECT_URI=https://localhost:8181/admin/login
```

### Step 3.3: Update the Flynn Site Yahoo OAuth Settings (Optional)
If you also want Yahoo login for the Flynn site, find these lines (around line 103-105):
```
# --- Yahoo OAuth ---
FLYNN_YAHOO_AUTH_CLIENT_ID=dummy_yahoo_client_id_flynn
FLYNN_YAHOO_AUTH_CLIENT_SECRET=dummy_yahoo_client_secret_flynn
FLYNN_YAHOO_AUTH_REDIRECT_URI=https://localhost:8182/admin/login
```

You can either:
- **Use the same Yahoo app** (if you added both redirect URIs in Step 2.3)
- **Create a separate Yahoo app** for the Flynn site with its own Client ID and Secret

### Step 3.4: Add Your Yahoo Email to Authorized Emails
Find the authorized emails line for the site you're configuring:

**For GG site (around line 67):**
```
GG_ADMIN_AUTHORIZED_EMAILS=jflynnpics@gmail.com
```

Add your Yahoo email to the comma-separated list:
```
GG_ADMIN_AUTHORIZED_EMAILS=jflynnpics@gmail.com,yourname@yahoo.com
```

**For Flynn site (around line 108):**
```
FLYNN_ADMIN_AUTHORIZED_EMAILS=jflynnpics@gmail.com
```

Add your Yahoo email:
```
FLYNN_ADMIN_AUTHORIZED_EMAILS=jflynnpics@gmail.com,yourname@yahoo.com
```

> **Important:** Your Yahoo email MUST be in the authorized emails list, otherwise login will fail with "email not authorized" error.

### Step 3.5: Save the File
Press `Ctrl+S` to save the changes.

---

## Part 4: Rebuild and Restart the Containers

### Step 4.1: Open Terminal
1. In VS Code, press `` Ctrl+` `` to open the integrated terminal
2. Make sure you're in the project root directory

### Step 4.2: Rebuild and Restart
Run the following command:
```bash
cd docker-compose && docker compose -f docker-compose.multi.yml --env-file .env.multi up --build -d api-gg api-flynn
```

This will:
1. Rebuild the API containers with the new configuration
2. Restart the containers in detached mode

### Step 4.3: Wait for Containers to Be Healthy
Wait about 30-60 seconds, then check the status:
```bash
cd docker-compose && docker compose -f docker-compose.multi.yml ps
```

Look for `healthy` in the STATUS column for both `artgallery-api-gg` and `artgallery-api-flynn`.

---

## Part 5: Test Yahoo OAuth Login

### Step 5.1: Navigate to the Admin Login Page
1. Open your browser
2. Go to: **http://localhost:8181/admin/login** (for GG site)
   or **http://localhost:8182/admin/login** (for Flynn site)

### Step 5.2: Click the Yahoo Login Button
1. You should see two login buttons:
   - **Sign in with Google**
   - **Sign in with Yahoo**
2. Click **Sign in with Yahoo**

### Step 5.3: Yahoo Authorization Page
1. You'll be redirected to Yahoo's authorization page
2. If you're not logged into Yahoo, sign in with your Yahoo credentials
3. You'll see a screen asking: **"Painting Gallery Admin would like to access your Yahoo account"**
4. It will request permission to access:
   - Your basic profile information
   - Your email address
5. Click **Allow** or **Accept**

### Step 5.4: Redirect Back to Admin Panel
1. After clicking Allow, Yahoo redirects you back to the admin login page
2. The application exchanges the authorization code for your profile
3. If your email is in the authorized list, you'll be logged in and redirected to the admin dashboard
4. If your email is NOT authorized, you'll see an error message

---

## Troubleshooting

### "Failed to get Yahoo authorization URL"
- Make sure the Yahoo OAuth credentials in `.env.multi` are correct
- Make sure the containers were rebuilt after changing the config
- Check container logs: `docker logs artgallery-api-gg`

### "Email not authorized"
- Your Yahoo email is not in the `GG_ADMIN_AUTHORIZED_EMAILS` list
- Add your Yahoo email to the authorized emails in `.env.multi`
- Rebuild and restart the containers

### Yahoo shows "Invalid redirect URI"
- The redirect URI in your Yahoo app settings doesn't match what the app is sending
- Make sure the redirect URI in Yahoo Developer Network exactly matches:
  - `https://localhost:8181/admin/login` (for GG)
  - `https://localhost:8182/admin/login` (for Flynn)

### Yahoo shows "Invalid client ID" or "Invalid client secret"
- Double-check you copied the Client ID and Client Secret correctly
- Make sure there are no extra spaces or hidden characters
- Try recreating the app in Yahoo Developer Network if the issue persists

---

## Summary of Files Modified

| File | What Changed |
|------|-------------|
| `docker-compose/.env.multi` | Replaced dummy Yahoo OAuth credentials with real values |
| `docker-compose/.env.multi` | Added Yahoo email to authorized emails list |

## Summary of URLs

| Purpose | URL |
|---------|-----|
| Yahoo Sign Up | https://login.yahoo.com/account/create |
| Yahoo Developer Network | https://developer.yahoo.com/apps/create/ |
| GG Admin Login | http://localhost:8181/admin/login |
| Flynn Admin Login | http://localhost:8182/admin/login |
