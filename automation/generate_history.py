import os
import random
import subprocess
from datetime import datetime, timedelta

REPO_PATH = r"F:\TravelSphere\.Net\MVC"

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
"email": "vivekhadiya168@gmail.com"
}
]

commit_messages = [
"Created HomeController",
"Added login validation",
"Improved navbar responsiveness",
"Created package model",
"Added booking CRUD functionality",
"Implemented logout feature",
"Added session handling",
"Created Register view",
"Improved package card UI",
"Fixed authentication issue",
"Added admin dashboard section",
"Created booking model",
"Added package update functionality",
"Improved profile validation",
"Added database configuration",
"Created login form UI",
"Added traveller profile page",
"Fixed booking validation issue",
"Added package filtering",
"Improved dashboard styling",
"Created PackageController",
"Added package delete method",
"Implemented role based access",
"Improved mobile responsiveness",
"Added review functionality",
"Created wishlist feature",
"Improved admin layout",
"Added booking history page",
"Fixed session timeout bug",
"Optimized database queries",
"Added forgot password UI",
"Created Budget Planner page",
"Added package details page",
"Improved form validation",
"Added notification module",
"Fixed navbar alignment issue",
"Created admin package table",
"Added traveller dashboard",
"Improved booking workflow",
"Refactored authentication logic",
"Added package search feature",
"Improved homepage UI",
"Fixed CSS responsiveness issue",
"Added profile update feature",
"Improved admin statistics section",
"Added review validation",
"Created dynamic package cards",
"Improved sidebar navigation",
"Added session expiration handling",
"Final UI cleanup and optimization"
]

start_date = datetime(2026, 2, 20)
end_date = datetime(2026, 4, 28)

current_date = start_date

os.chdir(REPO_PATH)

commit_count = 85

for i in range(commit_count):
    contributor = contributors[i % len(contributors)]

random_hour = random.randint(10, 20)
random_minute = random.randint(0, 59)

commit_date = current_date.replace(
    hour=random_hour,
    minute=random_minute
)

commit_message = random.choice(commit_messages)

dummy_file = os.path.join(REPO_PATH, "commit_log.txt")

with open(dummy_file, "a", encoding="utf-8") as f:
    f.write(f"{commit_date} - {commit_message}\n")

subprocess.run(
    ["git", "config", "user.name", contributor["name"]],
    check=True
)

subprocess.run(
    ["git", "config", "user.email", contributor["email"]],
    check=True
)

subprocess.run(["git", "add", "."], check=True)

env = os.environ.copy()

env["GIT_AUTHOR_DATE"] = commit_date.strftime("%Y-%m-%d %H:%M:%S")
env["GIT_COMMITTER_DATE"] = commit_date.strftime("%Y-%m-%d %H:%M:%S")

subprocess.run(
    ["git", "commit", "-m", commit_message],
    check=True,
    env=env
)

if random.choice([True, False]):
    current_date += timedelta(days=1)

subprocess.run(["git", "branch", "-M", "main"], check=True)

subprocess.run(
["git", "push", "-u", "origin", "main", "--force"],
check=True
)

print("Realistic commit history generated successfully!")
