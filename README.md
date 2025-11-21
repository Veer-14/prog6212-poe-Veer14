Report
1. Purpose

The Contract Monthly Claim System tries to make the processing of Independent Contractor (IC) lecturer claims easier within an institution by allowing lecturers to submit monthly claims for their work hours, attach supporting files, and track the progress of their claims. Programme Coordinators and Academic Managers review and verify before approving or rejecting claims, thus maintaining openness and accountability.

2. Users / Roles

Lecturer
•	Submits new monthly claims (Hours × Rate).
•	Uploads supporting files.
•	Views claim history and approval status.

Coordinator
•	Reviews all pending lecturer claims.
•	Verifies, approves or rejects the claims.
•	Adds review comments explaining decisions.

Manager
•	Performs senior approval for verified claims.
•	Approves or rejects claims verified by Coordinators.
•	Can generate automated summary reports.


HR / Admin (Future Scope)
•	It can view summary dashboards displaying claims statistics.
•	Tracking of claim trends, approvals, and pending claims.

3. Core Workflow
•	The lecturer makes a new claim and uploads the necessary documentation.
•	The claim is submitted and automatically checked against defined rules.
•	Claims meeting criteria are auto-verified, while others are flagged for manual review.
•	Coordinator logs in, views pending claims, and then verifies or rejects claims.
•	Manager logs in and approves claims that are verified.
•	System updates claim status and stores review records.
•	Lecturer can see updated status and review comments.

4. Platform Change and High-Level Design Decisions

•	Previous Platform: WPF (.NET Desktop Application)
•	Prototype UI with rich controls featuring gradients and blurred backgrounds.
•	Single-user application for initial testing and UI validation.
•	New Platform: ASP.NET Core MVC (Web Application)
•	Multiuser web access with role-based authentication.
•	Entity Framework Core for database operations.
•	MVC pattern allows for separation of concerns and maintainability.
•	Responsive styling with consistent visuals for dashboard views across all roles.

5. Overview of MVC Implementation

Models
Claim.cs: This stores details about the claim, automated verification rules, and status.
Review.cs: Logs actions taken by a reviewer, comments, timestamps, and decision.
User.cs - Contains user's credentials, roles, and hourly rate.


Controllers
LecturerController: handles claim submission, auto-verification, calculates the total of amounts, and file upload.
CoordinatorController: Controls the process of verification, review, and rejection of claims.
ManagerController: Approves verified claims and creates summary reports automatically.
AccountController: Handles login, logout, and session management.
HRController: Adds and edits users and their information 

Views
LecturerDashboard.cshtml - provides the form for submitting claims, calculates instantly the total amount and displays claim history.
CoordinatorDashboard.cshtml — Displays claims that are pending verification with Approve/Reject options.
ManagerDashboard.cshtml — Displays the claims that have been verified and are ready for approval.
HRDashboard.cshtml (future scope) — Administrative overview of all claims.

Styling
CSS used to maintain consistent backgrounds, rounded buttons, hover effects, and responsive tables.

6. Part 3 Functionalities Added

1. Automated Claim Calculations
•	Total claim amount auto-calculated on the claim form: Hours Worked × Hourly Rate
•	Live updates displayed to the user while typing hours.



2. Automated Verification
Claims are automatically verified, if:
•	HoursWorked > 0 and ≤ 180
•	HourlyRate > 0 and < 100
•	TotalAmount < 10,000
Claims that fail these criteria are highlighted for Manual Review.

4. Automated Report Generator
•	The manager can generate summary reports automatically for all approved claims.
•	Reports include total claims, approved/rejected status, and submission trends.

5. HR / Admin Dashboard
•	Overview of all claims by month, status, and total payouts.
•	Can filter and search claims, allowing HR to see a history and trends in claims.

6. Login & Session Handling
•	ASP.NET Identity provides secure login and session management.
•	Role-based redirects enforce correct access to Lecturer, Coordinator, Manager, or HR dashboards.

7. Unit Testing
xUnit used with EF Core InMemory database.
Tests cover:
•	Claim creation and validation.
•	Auto-verification logic.
•	Coordinator and Manager approval/rejection workflow.
•	Review records created with the correct information.
•	Session-based role validation for controller actions.



8. Security and Privacy
•	Passwords are hashed and never stored in plain text.
•	Role-based authorization restricts access to views and actions.
•	File uploads are stored securely, with access strictly limited to the claim owner and authorized reviewers.
•	Audit logs maintain full trace of claim decisions.

9. Constraints and Validations
•	File types: PDF, JPG, PNG, DOCX
•	Max file size: 10MB
•	Claim fields (Month, Hours, Rate) are required
•	HourlyRate ≥ 0, HoursWorked ≥ 0
•	One claim per lecturer per month
•	Auto-verification rules applied, eliminating human error

10. Future Enhancements

•	Add search, sort, and filter features on dashboards.
•	E-mail notifications upon approval or rejection.
•	Export reports to CSV/PDF for Managers and HR. Managerial analytics dashboard showing insight into the total claims, approvals, and trends. Deployment with SSL for secure multi-user access.
