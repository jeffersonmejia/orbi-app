// Global UI helpers.
document.addEventListener('DOMContentLoaded', () => {
  if (document.body.classList.contains('modal-frame-page')) {
    document.querySelectorAll('form[action*="/Edit"]').forEach(form => {
      const action = new URL(form.action, window.location.origin);
      action.searchParams.set('modal', '1');
      form.action = action.toString();
    });

    document.querySelectorAll('a[href*="/Details/"], a[href*="/Edit/"]').forEach(link => {
      const href = new URL(link.href, window.location.origin);
      href.searchParams.set('modal', '1');
      link.href = href.toString();
    });
  }

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

  const recordModalElement = document.getElementById('crudRecordModal');
  const recordFrame = recordModalElement?.querySelector('[data-crud-record-frame]');
  const recordTitle = recordModalElement?.querySelector('[data-crud-record-modal-title]');
  const recordIcon = recordModalElement?.querySelector('[data-crud-record-modal-icon]');
  const recordModal = recordModalElement && window.bootstrap
    ? new window.bootstrap.Modal(recordModalElement)
    : null;

  if (recordModal && recordFrame) {
    let firstFrameLoad = true;
    let activeMode = 'details';

    document.querySelectorAll('a[href*="/Details/"], a[href*="/Edit/"]').forEach(link => {
      link.addEventListener('click', event => {
        const recordUrl = new URL(link.href, window.location.origin);
        activeMode = recordUrl.pathname.includes('/Edit/') ? 'edit' : 'details';
        recordUrl.searchParams.set('modal', '1');

        event.preventDefault();
        firstFrameLoad = true;
        recordFrame.src = recordUrl.toString();

        if (recordTitle) {
          recordTitle.textContent = activeMode === 'edit' ? 'Edit record' : 'Record details';
        }

        if (recordIcon) {
          recordIcon.className = activeMode === 'edit' ? 'bi bi-pencil-square' : 'bi bi-eye-fill';
        }

        recordModal.show();
      });
    });

    recordFrame.addEventListener('load', () => {
      if (firstFrameLoad) {
        firstFrameLoad = false;
        return;
      }

      let framePath = '';
      try {
        framePath = recordFrame.contentWindow.location.pathname;
      } catch {
        window.location.reload();
        return;
      }

      if (framePath.includes('/Edit/') && activeMode !== 'edit') {
        activeMode = 'edit';

        if (recordTitle) {
          recordTitle.textContent = 'Edit record';
        }

        if (recordIcon) {
          recordIcon.className = 'bi bi-pencil-square';
        }

        return;
      }

      if (activeMode !== 'edit') return;

      if (!framePath.includes('/Edit/')) {
        window.location.reload();
      }
    });

    recordModalElement.addEventListener('hidden.bs.modal', () => {
      firstFrameLoad = true;
      recordFrame.removeAttribute('src');
    });
  }
});
