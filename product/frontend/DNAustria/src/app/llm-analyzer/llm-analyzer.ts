import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { environment } from '../environment';

@Component({
  selector: 'app-llm-analyzer',
  imports: [CommonModule],
  templateUrl: './llm-analyzer.html',
  styleUrl: './llm-analyzer.css',
})
export class LlmAnalyzer {
  results: string | null = null;
  loading: boolean = false;

  analyze(text: string) {
    if (!text || !text.trim()) {
      this.results = 'Please enter some text to analyze.';
      return;
    }

    this.loading = true;
    this.results = null;

    const apiBaseUrl = environment.apiUrl.replace(/\/$/, '');

    fetch(`${apiBaseUrl}/api/events/llm`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ prompt: text }),
    })
      .then(async (res) => {
        if (!res.ok) {
          const errorText = await res.text();
          throw new Error(errorText || `HTTP ${res.status}`);
        }

        const contentType = res.headers.get('content-type') || '';
        let body: any;
        if (contentType.includes('application/json')) {
          body = await res.json();
          this.results = JSON.stringify(body, null, 2);
        } else {
          body = await res.text();
          this.results = body;
        }
      })
      .catch((err) => {
        console.error('LLM analyze error', err);
        this.results = 'Request failed: ' + (err && err.message ? err.message : String(err));
      })
      .finally(() => {
        this.loading = false;
      });
  }
}
