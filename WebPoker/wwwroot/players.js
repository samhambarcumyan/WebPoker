"use strict";

const actionsForm = document.querySelector("#player-actions");
const actionFields = actionsForm.querySelector("fieldset");
const idInput = actionsForm.elements.playerId;
const nicknameForm = document.querySelector("#nickname-form");
const passwordForm = document.querySelector("#password-form");
const statusMessage = document.querySelector("#player-status");
let selectedId = null;

function showStatus(message, isError = false) {
    statusMessage.textContent = message;
    statusMessage.classList.toggle("is-error", isError);
}

function closeEditors() {
    selectedId = null;
    for (const form of [nicknameForm, passwordForm]) {
        form.hidden = true;
        form.reset();
    }
}

idInput.addEventListener("input", () => {
    closeEditors();
    showStatus("");
});

async function openEditor(id, operation) {
    showStatus("Finding player…");
    const response = await fetch("/api/player", { cache: "no-store" });
    if (!response.ok) {
        showStatus(`Could not load players (HTTP ${response.status}).`, true);
        return;
    }

    const players = await response.json();
    const player = players.find(item => item.id === id);
    if (!player) {
        showStatus(`Player ${id} was not found.`, true);
        return;
    }

    selectedId = id;
    showStatus(player.nickname ? `Editing ${player.nickname} (ID ${id}).` : `Editing player ${id}.`);
    if (operation === "nickname") {
        nicknameForm.hidden = false;
        const input = nicknameForm.elements.newNickname;
        input.value = player.nickname ?? "";
        input.focus();
        input.select();
    } else {
        passwordForm.hidden = false;
        passwordForm.elements.currentPassword.focus();
    }
}

async function deletePlayer(id) {
    showStatus("Deleting player…");
    const response = await fetch(`/api/player/${id}`, { method: "DELETE" });
    if (response.ok) {
        actionsForm.reset();
        showStatus(`Player ${id} was deleted.`);
    } else if (response.status === 404) {
        showStatus(`Player ${id} was not found.`, true);
    } else {
        showStatus(`Could not delete player (HTTP ${response.status}).`, true);
    }
}

actionsForm.addEventListener("submit", async event => {
    event.preventDefault();
    if (actionFields.disabled) return;

    const id = idInput.valueAsNumber;
    const operation = event.submitter?.value ?? "nickname";
    if (operation === "delete" && !window.confirm(`Delete player with ID ${id}?`)) return;

    closeEditors();
    actionFields.disabled = true;
    try {
        if (operation === "delete") {
            await deletePlayer(id);
        } else {
            await openEditor(id, operation);
        }
    } catch {
        showStatus("Could not reach the server or read its response.", true);
    } finally {
        actionFields.disabled = false;
    }
});

async function saveChanges(event) {
    event.preventDefault();
    const form = event.currentTarget;
    if (selectedId === null || form.hidden || actionFields.disabled) return;

    const id = selectedId;
    const change = form.dataset.change;
    const payload = Object.fromEntries(new FormData(form));
    if (Object.values(payload).some(value => !value.trim())) {
        showStatus(change === "nickname" ? "Enter a nickname." : "Enter both passwords.", true);
        return;
    }

    const fields = form.querySelector("fieldset");
    actionFields.disabled = true;
    fields.disabled = true;
    showStatus(`Saving ${change}…`);
    try {
        const response = await fetch(`/api/player/${id}/${change}`, {
            method: "PATCH",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        if (response.ok) {
            closeEditors();
            showStatus(change === "nickname"
                ? `Nickname for player ${id} changed to ${payload.newNickname.trim()}.`
                : `Password for player ${id} was changed.`);
        } else if (response.status === 404) {
            closeEditors();
            showStatus(`Player ${id} was not found.`, true);
        } else if (response.status === 400) {
            showStatus(change === "password" ? "Current password is incorrect." : "Enter a valid nickname.", true);
        } else {
            showStatus(`Could not save ${change} (HTTP ${response.status}).`, true);
        }
    } catch {
        showStatus("Could not connect to the server.", true);
    } finally {
        actionFields.disabled = false;
        fields.disabled = false;
        if (form.hidden) idInput.focus();
    }
}

nicknameForm.addEventListener("submit", saveChanges);
passwordForm.addEventListener("submit", saveChanges);
