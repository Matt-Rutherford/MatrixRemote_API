<!-- src/routes/+layout.svelte -->
<div class="layout">
	<header>
		<nav>
			<a href="/">Home</a>
			<a href="/message">Message</a>
		</nav>
	</header>

	<main>
		<slot></slot>
		<!-- This is where page content will be injected -->
	</main>

	<footer>
		<p>&copy; 2024 MatrixRemote</p>
	</footer>
</div>

<style>
	.layout {
		max-width: 900px;
		margin: auto;
		padding: 1rem;
	}

	header {
		display: flex;
		justify-content: space-between;
	}

	nav a {
		margin: 0 10px;
		text-decoration: none;
	}

	footer {
		text-align: center;
		margin-top: 2rem;
	}
</style>
