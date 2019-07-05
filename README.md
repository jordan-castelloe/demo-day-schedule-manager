# Demo Day Schedule Manager

## The Problem
For NewForce demo day, we ran into a scheduling conundrum. We asked our students to prioritize which companies they most wanted to work for and tried to schedule them in 30 minute interview blocks based on those preferences. This alone would have been a logistical nightmare (for example, if Person A rated Company A as their first pick and perosn B rated Company A as their second pick, we'd want Person A to take priority in the schedule). To further complicate things, certain companies hardly got prioritized at all because students knew very little about them. We still wanted to schedule interviews with these companies, but we'd have to generate those schedules randomly. And, last but not least, we needed to make sure that our interview schedules lined up with _where_ students wanted to work (some students are willing to relocate, some aren't) and whether or not they met the company's degree requirements, if they had any.

## The Solution
This app takes into account student preferences and company availability and generates a demo day schedule for us.

This app allows users to:
- Create companies and specify their location and degree requirements
- Specify when companies are available to interview (for example, if they need to leave early, we can take that into account)
- Create students, specify whether they can relocate to a different area, and whether or not they have a bachelor's degree
- Create time blocks for interviews-- users control when each time block starts and stops
- Enter each student's top five preferred companies

Based on that information, the app will generate a schedule based on:
- Student preferences- it schedules all top picks first before moving on and trying to schedule second picks. Every student will get their first pick. _Almost_ every student gets their second pick, depending on availability.
- Company availability- If companies have to leave early or arrive late, the app schedules them accordingly
- Location- Students who are willing to relocate will be scheduled with companies that are out of town. Students who can't relocate will only be scheduled with local companies.
- Company requirements- if a company requires a bachelor's degree, only students with a bachelor's degree will be scheduled for that company's interview blocks.

The schedule is avaiable in three different formats. Users can toggle between formats in the schedule manager view:
1. Organized by student
1. Organized by company
1. Organized by time block


## Instructions
### Dependencies
1. Visual Studio 2019
1. SQL Server Express

### How to run
1. Run `git clone https://github.com/jordan-castelloe/demo-day-schedule-manager.git` to clone down this repo
1. From your command line, run `dotnet restore`
1. From your command line, run `start DemoDay.sln` to open the solution in Visual Studio
1. Create a new SQL Server Database. Name in `DemoDay`.
1. Open your package manager console and type the command `update-database` to update the database.
1. Press `Ctrl + F5` to run

### How to use
1. You don't need to register or log in-- right now, this is for everyone. (In a future version, I'd like to have students log in and enter preferences themselves.)
1. Click on "Students" in the nav bar. Create all of the students who will be demoing.
1. Click on "Companies" in the nav bar. Create all the companies who are coming to demo day.
1. Click on "Schedule Manager" in the nav bar. Click "Manage Time Slots". Add start times and end times for your interview blocks.
1. Go back to the "Companies" page. By default, each company is available in every time block. If you want to change a company's availability, click the "Edit" button beside a company's name and select the time blocks when they are avaialble.
1. To add student preferences, go back to "Students" and click "Add Preferences" beside each student's name.
1. To generate a schedule, click on "Schedule Manager".
1. To view different schedule formats (by student, by company, by time block) click on the buttons at the top of the Schedule Manager page.