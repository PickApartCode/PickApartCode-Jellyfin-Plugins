(function () {
  const pageId = '#suggestionsPage';

  async function loadSuggestions() {
    const rows = document.querySelector('#suggestionRows');
    rows.innerHTML = '';

    const suggestions = await ApiClient.getJSON(ApiClient.getUrl('Suggestions'));
    suggestions.forEach((item) => rows.appendChild(renderRow(item)));
  }

  function renderRow(item) {
    const tr = document.createElement('tr');
    tr.innerHTML = `
      <td>${item.UserName}</td>
      <td>${item.MediaType}</td>
      <td>${item.Name || item.SearchTerm}</td>
      <td>${item.Status}</td>
      <td>${item.IsDuplicate ? item.DuplicateReason : 'No'}</td>
      <td>
        <button is='emby-button' class='button-submit accept'>Approve</button>
        <button is='emby-button' class='button-submit reject'>Reject</button>
        <button is='emby-button' class='button-submit added'>Mark Added</button>
      </td>`;

    tr.querySelector('.accept').addEventListener('click', () => moderate(item.Id, 'Approved'));
    tr.querySelector('.reject').addEventListener('click', () => moderate(item.Id, 'Rejected'));
    tr.querySelector('.added').addEventListener('click', () => moderate(item.Id, 'Added'));

    return tr;
  }

  async function moderate(id, status) {
    await ApiClient.ajax({
      type: 'POST',
      url: ApiClient.getUrl(`Suggestions/${id}`),
      data: JSON.stringify({ Id: id, Status: status })
    });

    Dashboard.alert('Suggestion updated.');
    await loadSuggestions();
  }

  document.addEventListener('pageshow', async (event) => {
    const page = event.target;
    if (!page.matches(pageId)) {
      return;
    }

    await loadSuggestions();
  });
})();
