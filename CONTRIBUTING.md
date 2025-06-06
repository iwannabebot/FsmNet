# Contributing to SharpFsm

🎉 Thank you for your interest in contributing to **SharpFsm**! Whether you want to report a bug, propose a feature, improve documentation, or contribute code, your help is appreciated.

---

## 📦 What is SharpFsm?

SharpFsm is a lightweight, extensible C# state machine library supporting generic FSMs, Moore and Mealy machines, transition validation, side effects, audit tracking, fluent builders, and more.

---

## 🛠️ Project Setup

1. **Clone the repo:**

   ```bash
   git clone https://github.com/iwannabebot/SharpFsm.git
   cd SharpFsm
   ```

2. **Build and test the solution:**

   ```bash
   dotnet build
   dotnet test
   ```

3. **Restore tool dependencies (if any):**

   ```bash
   dotnet restore
   ```

---

## 🧪 Testing

All PRs must pass tests.

- Add new unit tests for any new features or bug fixes.
- Use clear, descriptive names for test methods.
- Follow Arrange–Act–Assert convention for readability.

Run tests locally:

```bash
dotnet test
```

---

## 🧑‍💻 Contribution Guidelines

### 🐛 Reporting Bugs

- Use [Issues](https://github.com/iwannabebot/SharpFsm/issues) with the "bug" label.
- Include steps to reproduce, expected behavior, and screenshots or stack traces when applicable.

### 💡 Suggesting Features

- Open an Issue with the "enhancement" label.
- Explain **why** the feature matters and how it might be used.

### 🔧 Submitting a Pull Request

1. **Fork** the repository.
2. **Create a feature branch**:

   ```bash
   git checkout -b feature/my-awesome-feature
   ```

3. Follow the coding conventions (C#, PascalCase, xml-docs for public APIs).
4. Include or update **unit tests**.
5. Run tests and ensure they pass.
6. Open a **pull request**:
   - Describe **what** and **why** clearly.
   - Reference related issues if applicable (`Closes #42`).
7. Be responsive to feedback in code reviews.

---

## 🧹 Code Style

- Prefer clarity over cleverness.
- Public APIs must have XML documentation.
- Use `var` only when the type is obvious.
- Avoid premature optimization.
- Respect `.editorconfig` and `.gitattributes` if present.

---

## 🔐 Commit & Branch Naming

- **Feature branches**: `feature/<name>`
- **Bugfix branches**: `bugfix/<name>`
- **Test branches**: `test/<name>`

Commit messages:
- Use imperative tone: `"Add state transition audit support"` (✅), not `"Added"` or `"Adding"` (❌).

---

## 📄 Licensing

By contributing, you agree that your contributions will be licensed under the MIT License.

---

## 🙌 Need Help?

Open an [Issue](https://github.com/iwannabebot/SharpFsm/issues), start a discussion, or reach out via GitHub if you're stuck or unsure where to start.
