const API = 'https://iotassettracking-webapp-backend-bnfjf5gpbdbhcpb5.southafricanorth-01.azurewebsites.net/api';
let allFirmware    = [];
let allDeviceTypes = [];

function showAlert(message, type = 'success') {
  const id = 'alert-' + Date.now();
  document.getElementById('alert-container').innerHTML = `
    <div class="alert alert-${type}" id="${id}">
      <span>${message}</span>
      <button class="alert-close" onclick="document.getElementById('${id}').remove()">✕</button>
    </div>`;
  if (type === 'success') setTimeout(() => document.getElementById(id)?.remove(), 4000);
}

async function loadFirmware() {
  try {
    const [firmware, types] = await Promise.all([
      fetch(`${API}/Firmware`).then(r => r.json()),
      fetch(`${API}/DeviceType`).then(r => r.json()),
    ]);
    allFirmware    = firmware;
    allDeviceTypes = types;
    renderTable();
    populateDeviceTypeSelect();
  } catch (e) {
    showAlert('Failed to load firmware: ' + e.message, 'error');
  }
}

function renderTable() {
  const tbody = document.getElementById('firmware-tbody');
  if (allFirmware.length === 0) {
    tbody.innerHTML = `<tr><td colspan="6" class="empty-state">No firmware found.</td></tr>`;
    return;
  }
  tbody.innerHTML = allFirmware.map(f => `
    <tr>
      <td>${f.firmwareId}</td>
      <td>${f.version}</td>
      <td>${f.deviceTypeName}</td>
      <td>${statusBadge(f.status)}</td>
      <td>${formatDate(f.releasedAt)}</td>
      <td>
        <div class="actions-cell">
          <button class="btn btn-secondary btn-sm" onclick="editFirmware(${f.firmwareId})">Edit</button>
          <button class="btn btn-danger btn-sm" onclick="deleteFirmware(${f.firmwareId}, '${f.version}')">Delete</button>
        </div>
      </td>
    </tr>`).join('');
}

function statusBadge(status) {
  const map = { Active: 'badge-success', Deprecated: 'badge-warning', Beta: 'badge-info' };
  return `<span class="badge ${map[status] || 'badge-neutral'}">${status}</span>`;
}

function populateDeviceTypeSelect() {
  const select  = document.getElementById('fw-device-type');
  const current = select.value;
  select.innerHTML = '<option value="">— Select Device Type —</option>';
  allDeviceTypes.forEach(dt => {
    const opt = new Option(dt.name, dt.deviceTypeId);
    select.appendChild(opt);
  });
  if (current) select.value = current;
}

function showAddForm() {
  document.getElementById('edit-id').value          = '';
  document.getElementById('fw-version').value       = '';
  document.getElementById('fw-device-type').value   = '';
  document.getElementById('fw-status').value        = 'Active';
  document.getElementById('form-title').textContent     = 'Add New Firmware';
  document.getElementById('save-btn-text').textContent  = 'Save Firmware';
  populateDeviceTypeSelect();
  document.getElementById('form-section').style.display = 'block';
}

function hideForm() {
  document.getElementById('form-section').style.display = 'none';
}

function editFirmware(id) {
  const f = allFirmware.find(f => f.firmwareId === id);
  if (!f) return;
  document.getElementById('edit-id').value             = f.firmwareId;
  document.getElementById('form-title').textContent    = `Edit Firmware — v${f.version}`;
  document.getElementById('save-btn-text').textContent = 'Save Changes';
  populateDeviceTypeSelect();
  document.getElementById('fw-version').value     = f.version;
  document.getElementById('fw-device-type').value = f.deviceTypeId;
  document.getElementById('fw-status').value      = f.status;
  document.getElementById('form-section').style.display = 'block';
  document.getElementById('form-section').scrollIntoView({ behavior: 'smooth' });
}

async function saveFirmware() {
  const version      = document.getElementById('fw-version').value.trim();
  const deviceTypeId = document.getElementById('fw-device-type').value;
  const status       = document.getElementById('fw-status').value;
  const editId       = document.getElementById('edit-id').value;

  if (!version)      { showAlert('Version is required.', 'error'); return; }
  if (!deviceTypeId) { showAlert('Device Type is required.', 'error'); return; }

  const payload = { version, deviceTypeId: parseInt(deviceTypeId), status };

  try {
    let res;
    if (editId) {
      payload.firmwareId = parseInt(editId);
      res = await fetch(`${API}/Firmware/${editId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      });
    } else {
      res = await fetch(`${API}/Firmware`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      });
    }

    if (!res.ok) throw new Error(await res.text());

    showAlert(editId ? 'Firmware updated successfully.' : 'Firmware added successfully.');
    hideForm();
    loadFirmware();
  } catch (e) {
    showAlert('Error: ' + e.message, 'error');
  }
}

async function deleteFirmware(id, version) {
  if (!confirm(`Delete firmware version "${version}"?`)) return;
  try {
    const res = await fetch(`${API}/Firmware/${id}`, { method: 'DELETE' });
    if (!res.ok) throw new Error(await res.text());
    showAlert(`Firmware v${version} deleted.`);
    loadFirmware();
  } catch (e) {
    showAlert('Error: ' + e.message, 'error');
  }
}

function formatDate(iso) {
  return new Date(iso).toLocaleDateString('en-ZA', { year: 'numeric', month: 'short', day: 'numeric' });
}

loadFirmware();
