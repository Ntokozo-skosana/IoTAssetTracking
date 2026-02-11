const API = 'https://iotassettracking-webapp-backend-bnfjf5gpbdbhcpb5.southafricanorth-01.azurewebsites.net/api';
let allGroups = [];

function showAlert(message, type = 'success') {
  const id = 'alert-' + Date.now();
  document.getElementById('alert-container').innerHTML = `
    <div class="alert alert-${type}" id="${id}">
      <span>${message}</span>
      <button class="alert-close" onclick="document.getElementById('${id}').remove()">✕</button>
    </div>`;
  if (type === 'success') setTimeout(() => document.getElementById(id)?.remove(), 4000);
}

async function loadGroups() {
  try {
    const res = await fetch(`${API}/DeviceGroup`);
    allGroups = await res.json();
    renderTable();
    populateParentSelect();
  } catch (e) {
    showAlert('Failed to load groups: ' + e.message, 'error');
  }
}

function renderTable() {
  const tbody = document.getElementById('groups-tbody');
  if (allGroups.length === 0) {
    tbody.innerHTML = `<tr><td colspan="5" class="empty-state">No groups found.</td></tr>`;
    return;
  }
  tbody.innerHTML = allGroups.map(g => `
    <tr>
      <td>${g.groupId}</td>
      <td>${g.name}</td>
      <td>${g.parentGroupName
        ? `<span class="badge badge-info">${g.parentGroupName}</span>`
        : `<span class="badge badge-neutral">Root</span>`}
      </td>
      <td>${formatDate(g.createdAt)}</td>
      <td>
        <div class="actions-cell">
          <button class="btn btn-secondary btn-sm" onclick="editGroup(${g.groupId})">Edit</button>
          <button class="btn btn-danger btn-sm" onclick="deleteGroup(${g.groupId}, '${g.name}')">Delete</button>
        </div>
      </td>
    </tr>`).join('');
}

function populateParentSelect(excludeId = null) {
  const select = document.getElementById('parent-group');
  const current = select.value;
  select.innerHTML = '<option value="">— None (Root Group) —</option>';
  allGroups
    .filter(g => g.groupId !== excludeId)
    .forEach(g => {
      const opt = document.createElement('option');
      opt.value = g.groupId;
      opt.textContent = g.parentGroupName ? `${g.parentGroupName} › ${g.name}` : g.name;
      select.appendChild(opt);
    });
  if (current) select.value = current;
}

function showAddForm() {
  document.getElementById('edit-id').value       = '';
  document.getElementById('group-name').value    = '';
  document.getElementById('parent-group').value  = '';
  document.getElementById('form-title').textContent     = 'Add New Group';
  document.getElementById('save-btn-text').textContent  = 'Save Group';
  populateParentSelect();
  document.getElementById('form-section').style.display = 'block';
}

function hideForm() {
  document.getElementById('form-section').style.display = 'none';
}

function editGroup(id) {
  const g = allGroups.find(g => g.groupId === id);
  if (!g) return;
  document.getElementById('edit-id').value             = g.groupId;
  document.getElementById('group-name').value          = g.name;
  document.getElementById('form-title').textContent    = `Edit Group — ${g.name}`;
  document.getElementById('save-btn-text').textContent = 'Save Changes';
  populateParentSelect(id);
  document.getElementById('parent-group').value = g.parentGroupId ?? '';
  document.getElementById('form-section').style.display = 'block';
  document.getElementById('form-section').scrollIntoView({ behavior: 'smooth' });
}

async function saveGroup() {
  const name     = document.getElementById('group-name').value.trim();
  const parentId = document.getElementById('parent-group').value;
  const editId   = document.getElementById('edit-id').value;

  if (!name) { showAlert('Group name is required.', 'error'); return; }

  const payload = { name, parentGroupId: parentId ? parseInt(parentId) : null };

  try {
    let res;
    if (editId) {
      payload.groupId = parseInt(editId);
      res = await fetch(`${API}/DeviceGroup/${editId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      });
    } else {
      res = await fetch(`${API}/DeviceGroup`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      });
    }

    if (!res.ok) throw new Error(await res.text());

    showAlert(editId ? 'Group updated successfully.' : 'Group created successfully.');
    hideForm();
    loadGroups();
  } catch (e) {
    showAlert('Error: ' + e.message, 'error');
  }
}

async function deleteGroup(id, name) {
  if (!confirm(`Delete group "${name}"?`)) return;
  try {
    const res = await fetch(`${API}/DeviceGroup/${id}`, { method: 'DELETE' });
    if (!res.ok) throw new Error(await res.text());
    showAlert(`Group "${name}" deleted.`);
    loadGroups();
  } catch (e) {
    showAlert('Error: ' + e.message, 'error');
  }
}

function formatDate(iso) {
  return new Date(iso).toLocaleDateString('en-ZA', { year: 'numeric', month: 'short', day: 'numeric' });
}

loadGroups();
