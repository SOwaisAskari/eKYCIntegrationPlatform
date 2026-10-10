const $ = id => document.getElementById(id);
async function api(url, options = {}) {
  const response = await fetch(url, { headers: { "Content-Type": "application/json", ...(options.headers || {}) }, ...options });
  const text = await response.text();
  const data = text ? JSON.parse(text) : null;
  if (!response.ok) throw new Error(data?.message || data?.title || `Request failed (${response.status})`);
  return data;
}
const esc = value => String(value ?? "").replace(/[&<>"']/g, c => ({ "&":"&amp;","<":"&lt;",">":"&gt;",'"':"&quot;","'":"&#39;" }[c]));
const date = value => value ? new Date(value).toLocaleString() : "—";
function status(value) { return `<span class="status ${esc(String(value).toLowerCase())}">${esc(value)}</span>`; }
async function load() {
  try {
    const [requests, consents] = await Promise.all([api("/api/kyc/requests"), api("/api/consent")]);
    $("total").textContent = requests.length;
    $("pending").textContent = requests.filter(x => ["Pending","Retry","Processing"].includes(x.status)).length;
    $("completed").textContent = requests.filter(x => x.status === "Completed").length;
    $("consents").textContent = consents.filter(x => x.isGranted && x.purpose === "KycProcessing").length;
    $("requestRows").innerHTML = requests.length ? requests.map(x => `<tr>
      <td><strong>#KYC-${x.id}</strong></td><td>${esc(x.customerReference)}</td><td>${esc(x.identityType)}</td>
      <td>${status(x.status)}</td><td>${x.retryAttempts} / ${x.maxRetryAttempts}</td><td>${date(x.updatedAtUtc)}</td>
      <td><button class="tiny-btn" onclick="viewRequest(${x.id})">Details</button></td></tr>`).join("") :
      '<tr><td colspan="7" class="empty">No requests yet. Grant consent and submit a demo request.</td></tr>';
    $("consentRows").innerHTML = consents.length ? consents.map(x => `<tr>
      <td><strong>${esc(x.customerReference)}</strong></td><td>${esc(x.purpose)}</td><td>${status(x.isGranted ? "Granted" : "Revoked")}</td>
      <td>${date(x.updatedAtUtc)}</td><td>${x.isGranted ? `<button class="tiny-btn" onclick="revokeConsent(${x.id})">Revoke</button>` : "—"}</td></tr>`).join("") :
      '<tr><td colspan="5" class="empty">No consent records yet.</td></tr>';
  } catch (e) { showMessage(e.message, true); }
}
function showMessage(message, error = false) {
  const el = $("formMessage"); el.hidden = false; el.textContent = message; el.className = "message" + (error ? " error" : "");
}
$("consentForm").addEventListener("submit", async event => {
  event.preventDefault();
  try {
    await api("/api/consent", { method:"POST", body:JSON.stringify({ customerReference:$("consentCustomer").value.trim(), purpose:$("purpose").value.trim() }) });
    $("customer").value = $("consentCustomer").value.trim();
    showMessage("Consent granted. You can now submit a request for this customer.");
    await load();
  } catch (e) { showMessage(e.message, true); }
});
$("kycForm").addEventListener("submit", async event => {
  event.preventDefault();
  try {
    const item = await api("/api/kyc/requests", { method:"POST", body:JSON.stringify({
      customerReference:$("customer").value.trim(), identityType:$("identityType").value,
      identityNumber:$("identity").value.trim(), requester:$("requester").value.trim(), isLegacy:$("legacy").checked
    })});
    showMessage(`Request #KYC-${item.id} queued successfully. Start the eKYC.Worker project to process it.`);
    $("identity").value = "";
    await load();
  } catch (e) { showMessage(e.message, true); }
});
async function revokeConsent(id) {
  if (!confirm("Revoke this demo consent?")) return;
  try { await api(`/api/consent/${id}/revoke`, { method:"POST", body:"{}" }); await load(); }
  catch(e) { showMessage(e.message, true); }
}
async function viewRequest(id) {
  try {
    const item = await api(`/api/kyc/requests/${id}`);
    const events = (item.events || []).map(x => `${x.status} — ${x.message} (${date(x.createdAtUtc)})`).join("\n");
    alert(`KYC Request #${item.id}\nCustomer: ${item.customerReference}\nStatus: ${item.status}\nRetries: ${item.retryAttempts}/${item.maxRetryAttempts}\nMessage: ${item.message}\n\nHistory:\n${events || "No events"}`);
  } catch(e) { showMessage(e.message, true); }
}
$("refresh").addEventListener("click", load);
load();
setInterval(load, 10000);
