// Global UI helpers.
document.addEventListener('DOMContentLoaded', () => {
  const skeletonWraps = document.querySelectorAll('.skeleton-wrap');

  if (skeletonWraps.length) {
    setTimeout(() => {
      skeletonWraps.forEach(wrap => wrap.classList.add('hidden'));
      document.querySelectorAll('.content-real').forEach(el => el.classList.add('revealed'));
    }, 500);
  }

  document.querySelectorAll('[data-pagination-select]').forEach(select => {
    const populatePages = () => {
      if (select.dataset.loaded === 'true') return;

      const currentPage = Number(select.dataset.currentPage || '1');
      const totalPages = Number(select.dataset.totalPages || '1');
      const template = select.dataset.pageUrlTemplate;

      if (!template || totalPages <= 1) return;

      const fragment = document.createDocumentFragment();
      for (let page = 1; page <= totalPages; page += 1) {
        const option = document.createElement('option');
        option.value = template.replace('__page__', page);
        option.textContent = `Page ${page} of ${totalPages}`;
        option.selected = page === currentPage;
        fragment.appendChild(option);
      }

      select.replaceChildren(fragment);
      select.dataset.loaded = 'true';
    };

    select.addEventListener('focus', populatePages, { once: true });
    select.addEventListener('pointerdown', populatePages, { once: true });
    select.addEventListener('change', () => {
      if (select.value) window.location.href = select.value;
    });
  });

  document.querySelectorAll('[data-password-toggle]').forEach(button => {
    button.addEventListener('click', () => {
      const field = button.closest('.password-field');
      const input = field?.querySelector('[data-password-input]');
      const icon = button.querySelector('i');

      if (!input) return;

      const shouldShow = input.type === 'password';
      input.type = shouldShow ? 'text' : 'password';
      button.setAttribute('aria-label', shouldShow ? 'Hide password' : 'Show password');

      if (icon) {
        icon.classList.toggle('bi-eye-fill', !shouldShow);
        icon.classList.toggle('bi-eye-slash-fill', shouldShow);
      }
    });
  });
});
