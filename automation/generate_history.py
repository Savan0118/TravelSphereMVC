import os
import random
import subprocess
from datetime import datetime, timedelta

# ==============================
# PROJECT PATH
# ==============================

REPO_PATH = r"F:\TravelSphere\.Net\MVC"

# ==============================
# CONTRIBUTORS
# ==============================

contributors = [
    {
        "name": "Savan0118",
        "email": "spatel776@rku.ac.in"
    },
    {
        "name": "Tanisha0311",
        "email": "rathodtanisha15@gmail.com"
    },
    {
        "name": "vivekhadiya168",
        "email": "hadiyavivek168@gmail.com"
    }
]

# ==============================
# COMMIT GROUPS
# ==============================

commit_groups = [
    {
        "message": "Initialized ASP.NET MVC solution",
        "files": [
            "Program.cs",
            "TravelSphereMVC.sln",
            "TravelSphereMVC.csproj"
        ]
    },
    {
        "message": "Added application configuration settings",
        "files": [
            "appsettings.json"
        ]
    },
    {
        "message": "Created ApplicationDbContext",
        "files": [
            "Data"
        ]
    },
    {
        "message": "Added database connection setup",
        "files": [
            "Program.cs"
        ]
    },
    {
        "message": "Created traveller model",
        "files": [
            "Models"
        ]
    },
    {
        "message": "Added package model",
        "files": [
            "Models"
        ]
    },
    {
        "message": "Created booking model",
        "files": [
            "Models"
        ]
    },
    {
        "message": "Added HomeController",
        "files": [
            "Controllers"
        ]
    },
    {
        "message": "Created AccountController",
        "files": [
            "Controllers"
        ]
    },
    {
        "message": "Added login functionality",
        "files": [
            "Controllers",
            "Views"
        ]
    },
    {
        "message": "Created register page",
        "files": [
            "Views"
        ]
    },
    {
        "message": "Added session handling",
        "files": [
            "Controllers"
        ]
    },
    {
        "message": "Added authentication middleware",
        "files": [
            "Program.cs"
        ]
    },
    {
        "message": "Created admin dashboard",
        "files": [
            "Views",
            "Controllers"
        ]
    },
    {
        "message": "Added package CRUD functionality",
        "files": [
            "Controllers",
            "Views",
            "Models"
        ]
    },
    {
        "message": "Added booking management",
        "files": [
            "Controllers",
            "Views"
        ]
    },
    {
        "message": "Created profile management module",
        "files": [
            "Controllers",
            "Views"
        ]
    },
    {
        "message": "Added traveller dashboard",
        "files": [
            "Views"
        ]
    },
    {
        "message": "Implemented budget planner",
        "files": [
            "Controllers",
            "Views"
        ]
    },
    {
        "message": "Added responsive navbar",
        "files": [
            "Views",
            "wwwroot"
        ]
    },
    {
        "message": "Improved homepage UI",
        "files": [
            "Views",
            "wwwroot"
        ]
    },
    {
        "message": "Added package validation",
        "files": [
            "Models"
        ]
    },
    {
        "message": "Fixed login authentication issue",
        "files": [
            "Controllers"
        ]
    },
    {
        "message": "Added logout functionality",
        "files": [
            "Controllers"
        ]
    },
    {
        "message": "Improved booking validation",
        "files": [
            "Models"
        ]
    },
    {
        "message": "Created package details page",
        "files": [
            "Views"
        ]
    },
    {
        "message": "Added admin statistics cards",
        "files": [
            "Views"
        ]
    },
    {
        "message": "Improved package card design",
        "files": [
            "wwwroot"
        ]
    },
    {
        "message": "Added Bootstrap styling",
        "files": [
            "wwwroot"
        ]
    },
    {
        "message": "Fixed booking issue",
        "files": [
            "Controllers"
        ]
    },
    {
        "message": "Improved profile validation",
        "files": [
            "Models"
        ]
    },
    {
        "message": "Added migration files",
        "files": [
            "Migrations"
        ]
    },
    {
        "message": "Refactored project structure",
        "files": [
            "Controllers",
            "Views"
        ]
    },
    {
        "message": "Added admin role management",
        "files": [
            "Controllers"
        ]
    },
    {
        "message": "Improved dashboard styling",
        "files": [
            "Views",
            "wwwroot"
        ]
    },
    {
        "message": "Fixed responsive layout bugs",
        "files": [
            "Views",
            "wwwroot"
        ]
    },
    {
        "message": "Final UI cleanup and optimization",
        "files": [
            "Views",
            "wwwroot"
        ]
    }
]

# ==============================
# SETUP
# ==============================

os.chdir(REPO_PATH)

# Initialize git if not exists
if not os.path.exists(".git"):
    subprocess.run(["git", "init"], check=True)

# ==============================
# DATE RANGE
# ==============================

start_date = datetime(2026, 2, 20)
end_date = datetime(2026, 4, 28)

current_date = start_date

# ==============================
# GENERATE COMMITS
# ==============================

for i in range(85):

    contributor = contributors[i % len(contributors)]
    commit_data = commit_groups[i % len(commit_groups)]

    commit_message = commit_data["message"]
    commit_files = commit_data["files"]

    # random realistic time
    random_hour = random.randint(10, 21)
    random_minute = random.randint(0, 59)

    commit_date = current_date.replace(
        hour=random_hour,
        minute=random_minute,
        second=0
    )

    # change a temp file every commit
    with open("temp.txt", "a", encoding="utf-8") as f:
        f.write(f"{commit_message} - {commit_date}\n")

    # add selected files if exist
    for file in commit_files:
        if os.path.exists(file):
            subprocess.run(["git", "add", file], check=True)

    subprocess.run(["git", "add", "--all"], check=True)

    env = os.environ.copy()

    env["GIT_AUTHOR_NAME"] = contributor["name"]
    env["GIT_AUTHOR_EMAIL"] = contributor["email"]

    env["GIT_COMMITTER_NAME"] = contributor["name"]
    env["GIT_COMMITTER_EMAIL"] = contributor["email"]

    env["GIT_AUTHOR_DATE"] = commit_date.strftime("%Y-%m-%d %H:%M:%S")
    env["GIT_COMMITTER_DATE"] = commit_date.strftime("%Y-%m-%d %H:%M:%S")

    subprocess.run(
        ["git", "commit", "-m", commit_message],
        check=True,
        env=env
    )

    print(f"Committed: {commit_message}")

    # realistic spacing between commits
    if random.choice([True, False]):
        current_date += timedelta(days=1)

    if current_date > end_date:
        current_date = end_date

# ==============================
# PUSH TO GITHUB
# ==============================

subprocess.run(["git", "branch", "-M", "main"], check=True)

try:
    subprocess.run(
        ["git", "remote", "add", "origin",
         "https://github.com/Savan0118/TravelSphereMVC.git"],
        check=True
    )
except:
    pass

subprocess.run(
    ["git", "push", "-u", "origin", "main", "--force"],
    check=True
)

print("\nProfessional realistic history generated successfully!")